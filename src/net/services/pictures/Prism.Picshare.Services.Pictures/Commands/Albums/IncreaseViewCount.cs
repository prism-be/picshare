// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCount.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Albums;

public record AddPictureToAlbum(Guid OrganisationId, Guid AlbumId, Guid PictureId) : IRequest<Album>;

public class AddPictureToAlbumHandler : IRequestHandler<AddPictureToAlbum, Album>
{
    private readonly DaprClient _daprClient;

    public AddPictureToAlbumHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Album> Handle(AddPictureToAlbum request, CancellationToken cancellationToken)
    {
        var album = await _daprClient.GetStateAlbumAsync(request.OrganisationId, request.AlbumId, cancellationToken);

        album.Pictures.Add(request.PictureId);

        await _daprClient.SaveStateAsync(album, cancellationToken);

        return album;
    }
}