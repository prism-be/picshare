// -----------------------------------------------------------------------
//  <copyright file = "UserRegister.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.User;

public class UserRegister : BaseServiceBusWorker<Domain.User>
{
    public UserRegister(ILogger<UserRegister> logger, IServiceProvider serviceProvider) : base(logger)
    {
    }

    public override string Queue => Topics.User.Register;

    internal override async Task ProcessMessageAsync(IMediator mediator, Domain.User payload)
    {
        await mediator.Send(new RegisterConfirmation(payload));
    }
}