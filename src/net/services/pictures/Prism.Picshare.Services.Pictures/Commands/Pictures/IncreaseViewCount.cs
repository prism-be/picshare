// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCount.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record IncreaseViewCount(Guid OrganisationId, Guid PictureId) : IRequest<Picture>;

public class IncreaseViewCountHandler : IRequestHandler<IncreaseViewCount, Picture>
{
    private readonly DaprClient _daprClient;

    public IncreaseViewCountHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Picture> Handle(IncreaseViewCount request, CancellationToken cancellationToken)
    {
        var picture = await _daprClient.GetStatePictureAsync(request.OrganisationId, request.PictureId, cancellationToken);

        picture.Views++;
        await _daprClient.SaveStateAsync(picture, cancellationToken);

        return picture;
    }
}