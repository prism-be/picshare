// -----------------------------------------------------------------------
//  <copyright file = "ThumbnailsGenerated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Pictures;

public class ThumbnailsGenerated
{
    private readonly IMediator _mediator;

    public ThumbnailsGenerated(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Pictures) + "." + nameof(ThumbnailsGenerated))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.ThumbnailsGenerated, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var entityReference = JsonSerializer.Deserialize<EntityReference>(mySbMsg);

        if (entityReference == null)
        {
            return;
        }

        await _mediator.Send(new SetPictureReady(entityReference.OrganisationId, entityReference.Id));
    }
}