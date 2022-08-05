// -----------------------------------------------------------------------
//  <copyright file = "PictureUploaded.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureUploaded : BaseServiceBusWorker<EntityReference>
{
    private readonly IMediator _mediator;
    private readonly PublisherClient _publisherClient;

    public PictureUploaded(ILogger<PictureUploaded> logger, IMediator mediator, PublisherClient publisherClient) : base(logger)
    {
        _publisherClient = publisherClient;
        _mediator = mediator;
    }

    protected override int MaxConcurrentCalls => 1;

    public override string Queue => Topics.Pictures.Uploaded;

    internal override async Task ProcessMessageAsync(EntityReference payload)
    {
        await _mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 150, 150, true));
        await _mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 960, 540, false));
        await _mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 1920, 1080, false));
        await _mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 3840, 2160, false));

        await _publisherClient.PublishEventAsync(Topics.Pictures.ThumbnailsGenerated, new EntityReference
        {
            Id = payload.Id,
            OrganisationId = payload.OrganisationId
        }, CancellationToken.None);
    }
}