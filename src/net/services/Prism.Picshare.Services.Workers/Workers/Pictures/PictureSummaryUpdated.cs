// -----------------------------------------------------------------------
//  <copyright file = "PictureSummaryUpdated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureSummaryUpdated : BaseServiceBusWorker<PictureSummary>
{
    public PictureSummaryUpdated(ILogger<PictureSummaryUpdated> logger, IServiceProvider serviceProvider) : base(logger)
    {
    }

    public override string Queue => Topics.Pictures.SummaryUpdated;

    internal override async Task ProcessMessageAsync(IMediator mediator, PictureSummary payload)
    {
        await mediator.Send(new UpdateFlowSummary(payload));
    }
}