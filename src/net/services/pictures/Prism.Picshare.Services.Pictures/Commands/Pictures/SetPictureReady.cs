﻿// -----------------------------------------------------------------------
//  <copyright file = "SetPictureReady.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record SetPictureReady(Guid OrganisationId, Guid PictureId) : IRequest<PictureSummary>;

public class SetPictureReadyHandler : IRequestHandler<SetPictureReady, PictureSummary>
{
    private readonly PublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public SetPictureReadyHandler(StoreClient storeClient, PublisherClient publisherClient)
    {
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<PictureSummary> Handle(SetPictureReady request, CancellationToken cancellationToken)
    {
        var picture = await _storeClient.GetStateAsync<Picture>(EntityReference.ComputeKey(request.OrganisationId, request.PictureId), cancellationToken);

        picture.Summary.Ready = true;
        await _storeClient.SaveStateAsync(picture.Key, picture, cancellationToken);
        await _publisherClient.PublishEventAsync(Topics.Pictures.SummaryUpdated, picture.Summary, cancellationToken);

        return picture.Summary;
    }
}