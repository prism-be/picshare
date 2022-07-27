// -----------------------------------------------------------------------
//  <copyright file = "PictureSeen.cs" company = "Prism">
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

public class PictureSeen
{
    private readonly IMediator _mediator;

    public PictureSeen(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Pictures) + "." + nameof(PictureSeen))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.Seen, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var picture = JsonSerializer.Deserialize<EntityReference>(mySbMsg);

        if (picture == null)
        {
            return;
        }

        await _mediator.Send(new IncreaseViewCount(picture.OrganisationId, picture.Id));
    }
}