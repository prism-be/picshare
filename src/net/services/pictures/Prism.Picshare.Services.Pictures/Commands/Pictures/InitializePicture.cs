// -----------------------------------------------------------------------
//  <copyright file = "InitializePicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record InitializePicture(Guid OrganisationId, Guid Owner, Guid PictureId, PictureSource Source) : IRequest<Picture>;

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
            Published = true,
            Owner = request.Owner
        };

        await _daprClient.SaveStateAsync(picture, cancellationToken);

        var flow = await _daprClient.GetStateFlowAsync(request.OrganisationId, cancellationToken);
        flow.Pictures.Insert(0, new PictureSummary
        {
            OrganisationId = request.OrganisationId,
            Id = request.PictureId
        });
        await _daprClient.SaveStateAsync(flow, cancellationToken);

        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Created, picture, cancellationToken);

        return picture;
    }
}