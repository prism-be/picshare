// -----------------------------------------------------------------------
//  <copyright file = "InitializePicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures;

public record InitializePicture(Guid OrganisationId, Guid Owner, Guid PictureId, PictureSource Source) : IRequest<Picture>;

public class InitializePictureHandler : IRequestHandler<InitializePicture, Picture>
{
    private readonly PublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public InitializePictureHandler(StoreClient storeClient, PublisherClient publisherClient)
    {
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<Picture> Handle(InitializePicture request, CancellationToken cancellationToken)
    {
        var picture = new Picture
        {
            OrganisationId = request.OrganisationId,
            Id = request.PictureId,
            Source = request.Source,
            Published = true,
            Owner = request.Owner,
            Summary = new PictureSummary
            {
                OrganisationId = request.OrganisationId,
                Id = request.PictureId
            }
        };

        await _storeClient.SaveStateAsync(picture, cancellationToken);

        var flow = await _storeClient.GetStateAsync<Flow>(request.OrganisationId.ToString(), cancellationToken);

        if (flow.Id == Guid.Empty)
        {
            flow.Id = request.OrganisationId;
        }

        flow.Pictures.Insert(0, new PictureSummary
        {
            OrganisationId = request.OrganisationId,
            Id = request.PictureId
        });
        await _storeClient.SaveStateAsync(request.OrganisationId.ToString(), flow, cancellationToken);

        await _publisherClient.PublishEventAsync(Topics.Pictures.Created, picture, cancellationToken);

        return picture;
    }
}