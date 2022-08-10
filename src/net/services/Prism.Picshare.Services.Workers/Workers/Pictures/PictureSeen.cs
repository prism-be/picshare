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
    public PictureSeen(ILogger<PictureSeen> logger, IServiceProvider serviceProvider) : base(logger)
    {
    }

    public override string Queue => Topics.Pictures.Seen;

    internal override async Task ProcessMessageAsync(IMediator mediator, EntityReference payload)
    {
        await mediator.Send(new IncreaseViewCount(payload.OrganisationId, payload.Id));
    }
}