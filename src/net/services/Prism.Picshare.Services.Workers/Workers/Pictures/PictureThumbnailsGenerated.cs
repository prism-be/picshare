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
    public PictureThumbnailsGenerated(ILogger<PictureThumbnailsGenerated> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    public override string Queue => Topics.Pictures.ThumbnailsGenerated;

    internal override async Task ProcessMessageAsync(IMediator mediator, EntityReference payload)
    {
        await mediator.Send(new SetPictureReady(payload.OrganisationId, payload.Id));
    }
}