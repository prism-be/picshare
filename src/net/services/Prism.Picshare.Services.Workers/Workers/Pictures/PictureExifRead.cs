// -----------------------------------------------------------------------
//  <copyright file = "PictureExifRead.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureExifRead : BaseServiceBusWorker<Picture>
{
    private readonly IMediator _mediator;

    public PictureExifRead(ILogger<PictureExifRead> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Pictures.ExifRead;

    internal override async Task ProcessMessageAsync(Picture payload)
    {
        await _mediator.Send(new GeneratePictureSummary(payload.OrganisationId, payload.Id, payload.Exifs));
    }
}