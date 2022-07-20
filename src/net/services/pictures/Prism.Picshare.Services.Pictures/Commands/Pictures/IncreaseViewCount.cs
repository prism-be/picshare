// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCount.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record IncreaseViewCount(Guid OrganisationId, Guid PictureId) : IRequest<Picture>;

public class IncreaseViewCountHandler : IRequestHandler<IncreaseViewCount, Picture>
{
    private readonly StoreClient _storeClient;

    public IncreaseViewCountHandler(StoreClient storeClient)
    {
        _storeClient = storeClient;
    }

    public async Task<Picture> Handle(IncreaseViewCount request, CancellationToken cancellationToken)
    {
        var picture = await _storeClient.GetStateAsync<Picture>(EntityReference.ComputeKey(request.OrganisationId, request.PictureId), cancellationToken);

        picture.Views++;
        await _storeClient.SaveStateAsync(picture.Key, picture, cancellationToken);

        return picture;
    }
}