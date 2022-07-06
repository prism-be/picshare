﻿// -----------------------------------------------------------------------
//  <copyright file = "InitializePicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record InitializePicture(Guid OrganisationId, Guid PictureId, PictureSource Source) : IRequest<Picture>;

public class InitializePictureHandler : IRequestHandler<InitializePicture, Picture>
{
    private readonly DaprClient _daprClient;

    public InitializePictureHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Picture> Handle(InitializePicture request, CancellationToken cancellationToken)
    {
        var picture = new Picture
        {
            OrganisationId = request.OrganisationId,
            Id = request.PictureId,
            Source = request.Source,
            Published = true
        };

        await _daprClient.SaveStateAsync(picture, cancellationToken);

        return picture;
    }
}