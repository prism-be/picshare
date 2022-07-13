// -----------------------------------------------------------------------
//  <copyright file = "SetPictureReady.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record SetPictureReady(Guid OrganisationId, Guid PictureId) : IRequest<PictureSummary>;

public class SetPictureReadyHandler : IRequestHandler<SetPictureReady, PictureSummary>
{
    private readonly DaprClient _daprClient;

    public SetPictureReadyHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<PictureSummary> Handle(SetPictureReady request, CancellationToken cancellationToken)
    {
        var picture = await _daprClient.GetStatePictureAsync(request.OrganisationId, request.PictureId, cancellationToken);
        picture.Summary.Ready = true;
        await _daprClient.SaveStateAsync(picture, cancellationToken);
        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.SummaryUpdated, picture.Summary, cancellationToken);
        return picture.Summary;
    }
}