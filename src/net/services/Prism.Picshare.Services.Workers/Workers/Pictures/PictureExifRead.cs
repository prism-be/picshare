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
    public PictureExifRead(ILogger<PictureExifRead> logger, IServiceProvider serviceProvider) : base(logger)
    {
    }

    public override string Queue => Topics.Pictures.ExifRead;

    internal override async Task ProcessMessageAsync(IMediator mediator, Picture payload)
    {
        await mediator.Send(new GeneratePictureSummary(payload.OrganisationId, payload.Id, payload.Exifs));
    }
}