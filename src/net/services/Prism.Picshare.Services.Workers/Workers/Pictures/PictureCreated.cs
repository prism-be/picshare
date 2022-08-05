// -----------------------------------------------------------------------
//  <copyright file = "PictureCreated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Authorization;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureCreated : BaseServiceBusWorker<Picture>
{
    private readonly IMediator _mediator;

    public PictureCreated(ILogger<PictureCreated> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Pictures.Created;

    internal override async Task ProcessMessageAsync(Picture payload)
    {
        await _mediator.Send(new ReadMetaData(payload.OrganisationId, payload.Id));
        await _mediator.Send(new AuthorizeUser(payload.OrganisationId, payload.Owner, payload.Id));
    }
}