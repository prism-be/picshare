// -----------------------------------------------------------------------
//  <copyright file = "UserRegister.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.User;

public class Register: ISimpleFunction
{
    private readonly IMediator _mediator;

    public Register(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(User) + "." + nameof(Register))]
    public async Task Run([ServiceBusTrigger(Topics.User.Register, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var user = JsonSerializer.Deserialize<Domain.User>(mySbMsg);

        if (user == null)
        {
            return;
        }

        await _mediator.Send(new RegisterConfirmation(user));
    }
}