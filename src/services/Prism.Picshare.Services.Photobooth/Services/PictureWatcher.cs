// -----------------------------------------------------------------------
//  <copyright file = "PictureWatcher.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Polly;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Photobooth.Services;

public class PictureWatcher : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly DaprClient _daprClient;
    private readonly string? _destinationPath;
    private readonly IHostEnvironment _env;
    private readonly ILogger<PictureWatcher> _logger;
    private string? _pictureSourcePath;
    private Timer? _timer;

    public PictureWatcher(ILogger<PictureWatcher> logger, IHostEnvironment env, IConfiguration config, DaprClient daprClient)
    {
        _logger = logger;
        _env = env;
        _config = config;
        _daprClient = daprClient;

        _destinationPath = Path.Combine(_env.ContentRootPath, "wwwroot", "pictures");
        Directory.CreateDirectory(_destinationPath);
    }

    public Guid OrganisationId { get; private set; }
    public Guid SessionId { get; private set; }

    public void CheckFiles(object? state)
    {
        var task = Task.Run(async () =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_pictureSourcePath))
                {
                    _logger.LogWarning("Cannot poll the source path");
                    return;
                }

                _logger.LogDebug("Checking new files in {path}", _pictureSourcePath);

                foreach (var file in Directory.GetFiles(_pictureSourcePath))
                {
                    _logger.LogInformation("New file found : {file}", file);
                    await ProcessPictureAsync(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occured when checking new files");
            }
        });

        task.Wait();
    }

    public async Task ProcessPictureAsync(string fullPath)
    {
        _logger.LogInformation("Processing a picture taken request: {fullPath}", fullPath);

        var photoboothPicture = new PhotoboothPicture
        {
            Id = Guid.NewGuid(),
            OrganisationId = OrganisationId,
            SessionId = SessionId,
            OriginalFileName = Path.GetFileName(fullPath)
        };

        byte[]? data = null;

        var retry = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, _ => TimeSpan.FromMilliseconds(250), OnRetry);

        try
        {
            await retry.ExecuteAsync(async () =>
            {
                data = await File.ReadAllBytesAsync(fullPath);
                var destination = Path.Combine(_destinationPath!, photoboothPicture.Id.ToString());
                File.Move(fullPath, destination);
                _logger.LogInformation("File moved to local storage: {destination}", destination);
            });
        }
        catch (IOException exception)
        {
            _logger.LogError(exception, "Error when reading the picture {photoboothPicture}", photoboothPicture.Id);
            return;
        }

        if (data == null)
        {
            return;
        }

        await _daprClient.PublishEventAsync(DaprConfiguration.Photobooth.PubSub, Topics.Photobooth.PictureTaken, photoboothPicture);

        var uploadPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), OnRetry);

        await uploadPolicy.ExecuteAsync(async () =>
        {
            await UploadFile(photoboothPicture, data);
        });
    }

    private void OnRetry(Exception ex, TimeSpan delay, int retryAttempt, Context _)
    {
        _logger.LogError(ex, "The policy needs a retry. Attempts: {attemps}, delay;: {delay}", retryAttempt, delay);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var organisationId = _config.GetValue<string>("PHOTOBOOTH_ORGANISATION");

        if (string.IsNullOrWhiteSpace(organisationId))
        {
            _logger.LogError("Environment variable PHOTOBOOTH_ORGANISATION not found, cannot continue.");

            throw new MissingConfigurationException("Environment variable PHOTOBOOTH_ORGANISATION not found, cannot continue.");
        }

        OrganisationId = Guid.Parse(organisationId);

        var sessionId = _config.GetValue<string>("PHOTOBOOTH_SESSION");

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            _logger.LogError("Environment variable PHOTOBOOTH_SESSION not found, cannot continue.");

            throw new MissingConfigurationException("Environment variable PHOTOBOOTH_SESSION not found, cannot continue.");
        }

        SessionId = Guid.Parse(sessionId);

        _pictureSourcePath = _config.GetValue<string>("PHOTOBOOTH_PICTURES_SOURCE");

        if (string.IsNullOrWhiteSpace(_pictureSourcePath))
        {
            _pictureSourcePath = Path.Combine(_env.ContentRootPath, "pictures-source");
            _logger.LogWarning("Environment variable PHOTOBOOTH_PICTURES_SOURCE not found, defaulting to {path}", _pictureSourcePath);
        }

        _logger.LogInformation("Starting a background processor on {path}", _pictureSourcePath);
        Directory.CreateDirectory(_pictureSourcePath);

        _timer = new Timer(CheckFiles);
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));

        _logger.LogInformation("Execute is done !");

        return Task.CompletedTask;
    }

    private async Task UploadFile(PhotoboothPicture photoboothPicture, byte[] data)
    {
        var dataBase64 = Convert.ToBase64String(data);

        var blobName = $"{OrganisationId}/{photoboothPicture.Id}/source";

        await _daprClient.InvokeBindingAsync("datastore", "create", dataBase64, new Dictionary<string, string>
        {
            {
                "blobName", blobName
            },
            {
                "fileName", blobName
            }
        });
        _logger.LogInformation("Picture uploaded on storage: {blobName}", blobName);

        await _daprClient.PublishEventAsync(DaprConfiguration.PubSub, Topics.Photobooth.PictureUploaded, photoboothPicture);
    }
}