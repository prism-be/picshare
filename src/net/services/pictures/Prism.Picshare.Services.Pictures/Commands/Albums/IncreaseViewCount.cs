// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCount.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Albums;

public record AddPictureToAlbum(Guid OrganisationId, Guid AlbumId, Guid PictureId) : IRequest<Album>;

public class AddPictureToAlbumHandler : IRequestHandler<AddPictureToAlbum, Album>
{
    private readonly StoreClient _storeClient;

    public AddPictureToAlbumHandler(StoreClient storeClient)
    {
        _storeClient = storeClient;
    }

    public async Task<Album> Handle(AddPictureToAlbum request, CancellationToken cancellationToken)
    {
        var album = await _storeClient.GetStateAsync<Album>(EntityReference.ComputeKey(request.OrganisationId, request.AlbumId), cancellationToken);

        album.Pictures.Add(request.PictureId);

        await _storeClient.SaveStateAsync(album.Key, album, cancellationToken);

        return album;
    }
}