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
    private readonly IMediator _mediator;

    public PictureSummaryUpdated(ILogger<PictureSummaryUpdated> logger, IMediator mediator) : base(logger)
    {
        _mediator = mediator;
    }

    public override string Queue => Topics.Pictures.SummaryUpdated;

    internal override async Task ProcessMessageAsync(PictureSummary payload)
    {
        await _mediator.Send(new UpdateFlowSummary(payload));
    }
}