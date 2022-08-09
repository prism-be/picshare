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
    public PictureCreated(ILogger<PictureCreated> logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
    }

    public override string Queue => Topics.Pictures.Created;

    internal override async Task ProcessMessageAsync(IMediator mediator, Picture payload)
    {
        await mediator.Send(new ReadMetaData(payload.OrganisationId, payload.Id));
        await mediator.Send(new AuthorizeUser(payload.OrganisationId, payload.Owner, payload.Id));
    }
}