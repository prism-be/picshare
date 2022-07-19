// -----------------------------------------------------------------------
//  <copyright file = "RelaunchPictureEvents.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Admin;

public record RelaunchPictureEvents(Guid OrganisationId, string Topic) : IRequest;

public class RelaunchPictureEventsHandler : IRequestHandler<RelaunchPictureEvents>
{
    private readonly IPublisherClient _publisherClient;
    private readonly IStoreClient _storeClient;

    public RelaunchPictureEventsHandler(IStoreClient storeClient, IPublisherClient daprClient)
    {
        _publisherClient = daprClient;
        _storeClient = storeClient;
    }

    public async Task<Unit> Handle(RelaunchPictureEvents request, CancellationToken cancellationToken)
    {
        var flow = await _storeClient.GetStateAsync<Flow>(request.OrganisationId.ToString(), cancellationToken);

        foreach (var pictureSummary in flow.Pictures)
        {
            var picture = await _storeClient.GetStateAsync<Picture>(pictureSummary.Key, cancellationToken);
            await _publisherClient.PublishEventAsync(request.Topic, picture, cancellationToken);
        }

        return Unit.Value;
    }
}