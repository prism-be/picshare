// -----------------------------------------------------------------------
//  <copyright file = "SetPictureName.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record SetPictureName(Guid OrganisationId, Guid PictureId, string Name) : IRequest<Picture>;

public class SetPictureNameHandler : IRequestHandler<SetPictureName, Picture>
{
    private readonly IPublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public SetPictureNameHandler(StoreClient storeClient, IPublisherClient publisherClient)
    {
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<Picture> Handle(SetPictureName request, CancellationToken cancellationToken)
    {
        var picture = await _storeClient.GetStateNullableAsync<Picture>(EntityReference.ComputeKey(request.OrganisationId, request.PictureId), cancellationToken)
                      ?? new Picture
                      {
                          OrganisationId = request.OrganisationId,
                          Id = request.PictureId
                      };

        picture.Name = request.Name;
        await _storeClient.SaveStateAsync(picture.Key, picture, cancellationToken);

        await _publisherClient.PublishEventAsync(Topics.Pictures.Updated, picture, cancellationToken);

        return picture;
    }
}