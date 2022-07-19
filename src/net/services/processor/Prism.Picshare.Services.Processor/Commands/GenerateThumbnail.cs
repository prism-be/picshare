// -----------------------------------------------------------------------
//  <copyright file = "GenerateThumbnail.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using ImageMagick;
using MediatR;
using Polly;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Extensions;

namespace Prism.Picshare.Services.Processor.Commands;

public record GenerateThumbnail(Guid OrganisationId, Guid PictureId, int Width, int Height, bool Crop) : IRequest<ResultCodes>;

public class GenerateThumbnailValidator : AbstractValidator<GenerateThumbnail>
{
    public GenerateThumbnailValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.PictureId).NotEmpty();
        RuleFor(x => x.Width).GreaterThan(0);
        RuleFor(x => x.Height).GreaterThan(0);
    }
}

public class GenerateThumbnailHandler : IRequestHandler<GenerateThumbnail, ResultCodes>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<GenerateThumbnailHandler> _logger;

    public GenerateThumbnailHandler(ILogger<GenerateThumbnailHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<ResultCodes> Handle(GenerateThumbnail request, CancellationToken cancellationToken)
    {
        var blobName = BlobNamesExtensions.GetSourcePath(request.OrganisationId, request.PictureId);

        var response = await _daprClient.ReadPictureAsync(blobName, cancellationToken);

        var pictureData = response.Data.ToArray();

        using var image = new MagickImage(pictureData);

        var width = request.Width;
        var height = request.Height;

        if (image.Height > image.Width)
        {
            width = request.Height;
            height = request.Width;
        }

        var ratio = (float)width / height;
        var sizeRatio = new MagickGeometry(image.Width,  Convert.ToInt32(image.Width * ratio));

        if (sizeRatio.Height > image.Height)
        {
            sizeRatio = new MagickGeometry(Convert.ToInt32(image.Height / ratio), image.Height);
        }

        if (request.Crop)
        {
            image.Crop(sizeRatio, Gravity.Center);
        }

        var size = new MagickGeometry(width, height);
        image.Resize(size);
        image.RePage();

        using var outputStream = new MemoryStream();
        image.Format = MagickFormat.Jpg;
        image.Quality = 95;
        await image.WriteAsync(outputStream, cancellationToken);

        var dataBase64 = Convert.ToBase64String(outputStream.ToArray());
        var blobNameResized = BlobNamesExtensions.GetSourcePath(request.OrganisationId, request.PictureId, request.Width, request.Height);

        var uploadPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), OnRetry);

        await uploadPolicy.ExecuteAsync(async () =>
        {
            await _daprClient.InvokeBindingAsync(Stores.Data, "create", dataBase64, new Dictionary<string, string>
            {
                {
                    "blobName", blobNameResized
                },
                {
                    "fileName", blobNameResized
                }
            }, cancellationToken);
            _logger.LogInformation("Picture uploaded on storage: {blobName}", blobNameResized);
        });

        return ResultCodes.Ok;
    }

    private void OnRetry(Exception ex, TimeSpan delay, int retryAttempt, Context _)
    {
        _logger.LogError(ex, "The policy needs a retry. Attempts: {attemps}, delay;: {delay}", retryAttempt, delay);
    }
}