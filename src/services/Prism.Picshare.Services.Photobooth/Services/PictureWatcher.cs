// -----------------------------------------------------------------------
//  <copyright file="PictureWatcher.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
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
    private readonly IHostEnvironment _env;
    private readonly ILogger<PictureWatcher> _logger;

    private string? _destinationPath;

    private FileSystemWatcher? _watcher;

    public PictureWatcher(ILogger<PictureWatcher> logger, IHostEnvironment env, IConfiguration config, DaprClient daprClient)
    {
        _logger = logger;
        _env = env;
        _config = config;
        _daprClient = daprClient;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SessionId { get; private set; }

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

        var path = _config.GetValue<string>("PHOTOBOOTH_PICTURES_SOURCE");

        if (string.IsNullOrWhiteSpace(path))
        {
            path = Path.Combine(_env.ContentRootPath, "pictures-source");
            _logger.LogWarning("Environment variable PHOTOBOOTH_PICTURES_SOURCE not found, defaulting to {path}", path);
        }

        _logger.LogInformation("Starting a background processor on {path}", path);
        Directory.CreateDirectory(path);

        _destinationPath = Path.Combine(_env.ContentRootPath, "wwwroot", "pictures");
        Directory.CreateDirectory(_destinationPath);

        _watcher = new FileSystemWatcher(path);
        _watcher.Created += NewPictureCreated;
        _watcher.EnableRaisingEvents = true;

        return Task.CompletedTask;
    }

    private void NewPictureCreated(object sender, FileSystemEventArgs e)
    {
        var task = ProcessPictureAsync(e.FullPath);
        task.Wait();
    }

    public async Task ProcessPictureAsync(string fullPath)
    {
        _logger.LogInformation("Processing a picture taken request: {fullPath}", fullPath);

        var photoboothPicture = new PhotoboothPicture
        {
            Id = Guid.NewGuid(), OrganisationId = OrganisationId, SessionId = SessionId, OriginalFileName = Path.GetFileName(fullPath)
        };

        var data = await File.ReadAllBytesAsync(fullPath);
        var destination = Path.Combine(_destinationPath!, photoboothPicture.Id.ToString());
        File.Move(fullPath, destination);
        _logger.LogInformation("File moved to local storage: {destination}", destination);
        
        await _daprClient.PublishEventAsync(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken, photoboothPicture);

        var policy = Policy.Handle<DaprException>()
            .RetryForever(onRetry: exception =>
            {
                _logger.LogError(exception, "Error when uploading the picture {photoboothPicture}", photoboothPicture.Id);
            });

        await policy.Execute(async () =>
        {
            await UploadFile(photoboothPicture, data);
        });
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