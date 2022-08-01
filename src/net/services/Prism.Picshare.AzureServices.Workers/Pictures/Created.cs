// -----------------------------------------------------------------------
//  <copyright file = "UserRegister.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Authorization;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Pictures;

public class Created : ISimpleFunction
{
    private readonly IMediator _mediator;

    public Created(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Pictures) + "." + nameof(Created))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.Created, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var picture = JsonSerializer.Deserialize<Picture>(mySbMsg);

        if (picture == null)
        {
            return;
        }

        await _mediator.Send(new ReadMetaData(picture.OrganisationId, picture.Id));
        await _mediator.Send(new AuthorizeUser(picture.OrganisationId, picture.Owner, picture.Id));
    }
}