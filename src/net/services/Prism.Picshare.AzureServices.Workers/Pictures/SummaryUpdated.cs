// -----------------------------------------------------------------------
//  <copyright file = "SummaryUpdated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Pictures;

public class SummaryUpdated : ISimpleFunction
{
    private readonly IMediator _mediator;

    public SummaryUpdated(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Pictures) + "." + nameof(SummaryUpdated))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.SummaryUpdated, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var summary = JsonSerializer.Deserialize<PictureSummary>(mySbMsg);

        if (summary == null)
        {
            return;
        }

        await _mediator.Send(new UpdateFlowSummary(summary));
    }
}