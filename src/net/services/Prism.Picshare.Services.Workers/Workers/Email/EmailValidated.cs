// -----------------------------------------------------------------------
//  <copyright file = "EmailValidated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Email;

public class EmailValidated : BaseServiceBusWorker<Domain.User>
{
    private readonly IMediator _mediator;

    public EmailValidated(ILogger<EmailValidated> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Email.Validated;

    internal override async Task ProcessMessageAsync(Domain.User payload)
    {
        await _mediator.Send(new EmailValidatedRequest(payload.OrganisationId, payload.Id));
    }
}