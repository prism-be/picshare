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
    private readonly IMediator _mediator;

    public UserRegister(ILogger<UserRegister> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.User.Register;

    internal override async Task ProcessMessageAsync(Domain.User payload)
    {
        await _mediator.Send(new RegisterConfirmation(payload));
    }
}