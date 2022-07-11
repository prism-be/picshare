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

namespace Prism.Picshare.Services.Processor.Commands;

public record GenerateThumbnail(Guid OrganisationId, Guid PictureId, int Width, int Height) : IRequest<ResultCodes>;

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
        var blobName = $"{request.OrganisationId}/{request.PictureId}/source";

        var bindingRequest = new BindingRequest(Stores.Data, "get");
        bindingRequest.Metadata.Add("blobName", blobName);
        bindingRequest.Metadata.Add("fileName", blobName);

        var response = await _daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);

        if (response == null)
        {
            _logger.LogInformation("No picture data found for the request : {request}", request);
            return ResultCodes.PictureNotFound;
        }

        var pictureData = response.Data.ToArray();

        using var image = new MagickImage(pictureData);
        var size = new MagickGeometry(request.Width, request.Height)
        {
            IgnoreAspectRatio = true
        };
        image.Resize(size);

        using var outputStream = new MemoryStream();
        await image.WriteAsync(outputStream, cancellationToken);

        var dataBase64 = Convert.ToBase64String(outputStream.ToArray());
        var blobNameResized = $"{request.OrganisationId}/{request.PictureId}/{request.Width}-{request.Height}";

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