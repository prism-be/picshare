// -----------------------------------------------------------------------
//  <copyright file = "PictureUploaded.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureUploaded : BaseServiceBusWorker<EntityReference>
{
    private readonly PublisherClient _publisherClient;

    public PictureUploaded(ILogger<PictureUploaded> logger, IServiceProvider serviceProvider, PublisherClient publisherClient) : base(logger)
    {
        _publisherClient = publisherClient;
    }

    public override string Queue => Topics.Pictures.Uploaded;

    protected override ushort PrefetchCount => 5;

    internal override async Task ProcessMessageAsync(IMediator mediator, EntityReference payload)
    {
        await mediator.Send(new CleanThumbnails(payload.OrganisationId, payload.Id));
        
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 150, 150, true));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 300, 300, true));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 960, 540, false));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 1280, 720, false));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 1920, 1080, false));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 2560, 1440, false));
        await mediator.Send(new GenerateThumbnail(payload.OrganisationId, payload.Id, 3840, 2160, false));

        await _publisherClient.PublishEventAsync(Topics.Pictures.ThumbnailsGenerated, new EntityReference
        {
            Id = payload.Id,
            OrganisationId = payload.OrganisationId
        }, CancellationToken.None);
    }
}