// -----------------------------------------------------------------------
//  <copyright file = "UploadPicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Polly;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

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
    private readonly DaprClient _daprClient;
    private readonly ILogger<UploadPictureHandler> _logger;

    public UploadPictureHandler(ILogger<UploadPictureHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<Unit> Handle(UploadPicture request, CancellationToken cancellationToken)
    {
        var uploadPolicy = Policy.Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), OnRetry);

        await uploadPolicy.ExecuteAsync(async () =>
        {
            await UploadFile(request);
        });

        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Uploaded, new EntityReference
        {
            OrganisationId = request.OrganisationId,
            Id = request.Id
        }, cancellationToken);

        return Unit.Value;
    }

    private void OnRetry(Exception ex, TimeSpan delay, int retryAttempt, Context _)
    {
        _logger.LogError(ex, "The policy needs a retry. Attempts: {attemps}, delay;: {delay}", retryAttempt, delay);
    }

    private async Task UploadFile(UploadPicture picture)
    {
        var dataBase64 = Convert.ToBase64String(picture.Data);

        var blobName = $"{picture.OrganisationId}/{picture.Id}/source";

        await _daprClient.InvokeBindingAsync(Stores.Data, "create", dataBase64, new Dictionary<string, string>
        {
            {
                "blobName", blobName
            },
            {
                "fileName", blobName
            }
        });
        _logger.LogInformation("Picture uploaded on storage: {blobName}", blobName);
    }
}