// -----------------------------------------------------------------------
//  <copyright file = "PictureSeen.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureSeen : BaseServiceBusWorker<EntityReference>
{
    private readonly IMediator _mediator;

    public PictureSeen(ILogger<PictureSeen> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Pictures.Seen;

    internal override async Task ProcessMessageAsync(EntityReference payload)
    {
        await _mediator.Send(new IncreaseViewCount(payload.OrganisationId, payload.Id));
    }
}