// -----------------------------------------------------------------------
//  <copyright file = "PictureThumbnailsGenerated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureThumbnailsGenerated : BaseServiceBusWorker<EntityReference>
{
    private readonly IMediator _mediator;

    public PictureThumbnailsGenerated(ILogger<PictureThumbnailsGenerated> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Pictures.ThumbnailsGenerated;

    internal override async Task ProcessMessageAsync(EntityReference payload)
    {
        await _mediator.Send(new SetPictureReady(payload.OrganisationId, payload.Id));
    }
}