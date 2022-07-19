// -----------------------------------------------------------------------
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
    private readonly IPublisherClient _publisherClient;
    private readonly IStoreClient _storeClient;

    public SetPictureReadyHandler(IStoreClient storeClient, IPublisherClient publisherClient)
    {
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<PictureSummary> Handle(SetPictureReady request, CancellationToken cancellationToken)
    {
        var picture = await _storeClient.GetStatePictureAsync(request.OrganisationId, request.PictureId, cancellationToken);

        picture.Summary.Ready = true;
        await _storeClient.SaveStateAsync(picture, cancellationToken);
        await _publisherClient.PublishEventAsync(Topics.Pictures.SummaryUpdated, picture.Summary, cancellationToken);

        return picture.Summary;
    }
}