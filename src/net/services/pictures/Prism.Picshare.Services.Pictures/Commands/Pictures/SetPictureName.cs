// -----------------------------------------------------------------------
//  <copyright file = "SetPictureName.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record SetPictureName(Guid OrganisationId, Guid PictureId, string Name) : IRequest<Picture>;

public class SetPictureNameHandler : IRequestHandler<SetPictureName, Picture>
{
    private readonly DaprClient _daprClient;

    public SetPictureNameHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Picture> Handle(SetPictureName request, CancellationToken cancellationToken)
    {
        var picture = await _daprClient.GetStatePictureAsync(request.OrganisationId, request.PictureId, cancellationToken);

        picture.Name = request.Name;
        await _daprClient.SaveStateAsync(picture, cancellationToken);
        
        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Updated, picture, cancellationToken);

        return picture;
    }
}