// -----------------------------------------------------------------------
//  <copyright file = "Validated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Email;

public class Validated
{
    private readonly IMediator _mediator;

    public Validated(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Email) + "." + nameof(Validated))]
    public async Task<ResultCodes> Run([ServiceBusTrigger(Topics.Email.Validated, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var user = JsonSerializer.Deserialize<Domain.User>(mySbMsg);

        if (user == null)
        {
            return ResultCodes.Unknown;
        }

        return await _mediator.Send(new EmailValidatedRequest(user.OrganisationId, user.Id));
    }
}