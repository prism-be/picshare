// -----------------------------------------------------------------------
//  <copyright file = "UserRegister.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.AzureServices.Workers.Mailing;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Processor;

public class PictureCreated : ISimpleFunction
{
    private readonly IMediator _mediator;

    public PictureCreated(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Processor) + "." + nameof(PictureCreated))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.Created, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var picture = JsonSerializer.Deserialize<EntityReference>(mySbMsg);

        if (picture == null)
        {
            return;
        }

        await _mediator.Send(new ReadMetaData(picture.OrganisationId, picture.Id));
    }
}