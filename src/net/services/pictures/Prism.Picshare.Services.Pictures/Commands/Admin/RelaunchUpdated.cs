// -----------------------------------------------------------------------
//  <copyright file = "RelaunchCreated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Commands.Admin;

public record RelaunchUpdated(Guid OrganisationId) : IRequest;

public class RelaunchUpdatedHandler : IRequestHandler<RelaunchUpdated>
{
    private readonly DaprClient _daprClient;

    public RelaunchUpdatedHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Unit> Handle(RelaunchUpdated request, CancellationToken cancellationToken)
    {
        var flow = await _daprClient.GetStateFlowAsync(request.OrganisationId, cancellationToken);

        foreach (var pictureSummary in flow.Pictures)
        {
            var picture = await _daprClient.GetStatePictureAsync(pictureSummary.OrganisationId, pictureSummary.Id, cancellationToken);
            await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Updated, picture, cancellationToken);
        }

        return Unit.Value;
    }
}