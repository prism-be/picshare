// -----------------------------------------------------------------------
//  <copyright file = "UploadPicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Extensions;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures;

public record UploadPicture(Guid OrganisationId, Guid Id, byte[] Data) : IRequest;

public class UploadPictureValidator : AbstractValidator<UploadPicture>
{
    public UploadPictureValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).NotEmpty().Must(x => x.Any(c => c != 0));
    }
}

public class UploadPictureHandler : IRequestHandler<UploadPicture>
{
    private readonly BlobClient _blobClient;
    private readonly ILogger<UploadPictureHandler> _logger;
    private readonly PublisherClient _publisherClient;

    public UploadPictureHandler(BlobClient blobClient, PublisherClient publisherClient, ILogger<UploadPictureHandler> logger)
    {
        _blobClient = blobClient;
        _publisherClient = publisherClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(UploadPicture request, CancellationToken cancellationToken)
    {
        var uploadPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), OnRetry);

        await uploadPolicy.ExecuteAsync(async () =>
        {
            await UploadFile(request);
        });

        await _publisherClient.PublishEventAsync(Topics.Pictures.Uploaded, new EntityReference
        {
            OrganisationId = request.OrganisationId,
            Id = request.Id
        }, cancellationToken);

        return Unit.Value;
    }

    private void OnRetry(Exception ex, TimeSpan delay, int retryAttempt, Context _)
    {
        _logger.LogError(ex, "The policy needs a retry. Attempts: {attemps}, delay: {delay}", retryAttempt, delay);
    }

    private async Task UploadFile(UploadPicture picture)
    {
        var blobName = BlobNamesExtensions.GetSourcePath(picture.OrganisationId, picture.Id);

        await _blobClient.CreateAsync(blobName, picture.Data);

        _logger.LogInformation("Picture uploaded on storage: {blobName}", blobName);
    }
}