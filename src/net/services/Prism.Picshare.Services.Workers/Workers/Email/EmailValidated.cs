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
    public EmailValidated(ILogger<EmailValidated> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    public override string Queue => Topics.Email.Validated;

    internal override async Task ProcessMessageAsync(IMediator mediator, Domain.User payload)
    {
        await mediator.Send(new EmailValidatedRequest(payload.OrganisationId, payload.Id));
    }
}