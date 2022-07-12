// -----------------------------------------------------------------------
//  <copyright file = "SummaryUpdatedController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class SummaryUpdatedController : Controller
{
    private readonly IMediator _mediator;

    public SummaryUpdatedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.SummaryUpdated)]
    [Topic(Publishers.PubSub, Topics.Pictures.SummaryUpdated)]
    public async Task<IActionResult> Execute([FromBody] PictureSummary summary)
    {
        var flow = await _mediator.Send(new UpdateFlowSummary(summary));
        return Ok(flow);
    }
}