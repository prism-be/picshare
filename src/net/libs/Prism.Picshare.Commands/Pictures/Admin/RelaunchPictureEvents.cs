﻿// -----------------------------------------------------------------------
//  <copyright file = "RelaunchPictureEvents.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures.Admin;

public record RelaunchPictureEvents(Guid OrganisationId, string Topic) : IRequest;

public class RelaunchPictureEventsHandler : IRequestHandler<RelaunchPictureEvents>
{
    private readonly PublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public RelaunchPictureEventsHandler(StoreClient storeClient, PublisherClient daprClient)
    {
        _publisherClient = daprClient;
        _storeClient = storeClient;
    }

    public async Task<Unit> Handle(RelaunchPictureEvents request, CancellationToken cancellationToken)
    {
        var flow = await _storeClient.GetStateAsync<Flow>(request.OrganisationId.ToString(), cancellationToken);
        var pictures = new List<Picture>();

        foreach (var pictureSummary in flow.Pictures)
        {
            var picture = await _storeClient.GetStateNullableAsync<Picture>(pictureSummary.OrganisationId, pictureSummary.Id, cancellationToken);

            if (picture == null)
            {
                continue;
            }

            if (picture.OrganisationId == Guid.Empty)
            {
                picture.OrganisationId = pictureSummary.OrganisationId;
                await _storeClient.SaveStateAsync(picture, cancellationToken);
            }

            pictures.Add(picture);
        }

        await _publisherClient.PublishEventsAsync(request.Topic, pictures, cancellationToken);

        return Unit.Value;
    }
}