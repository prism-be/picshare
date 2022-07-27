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

namespace Prism.Picshare.AzureServices.Workers.Mailing;

public class UserRegister
{
    private readonly IMediator _mediator;

    public UserRegister(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Mailing) + "." + nameof(UserRegister))]
    public async Task Run([ServiceBusTrigger(Topics.User.Register, Topics.Subscription, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var user = JsonSerializer.Deserialize<Domain.User>(mySbMsg);

        if (user == null)
        {
            return;
        }

        await _mediator.Send(new RegisterConfirmation(user));
    }
}