// -----------------------------------------------------------------------
//  <copyright file = "ManagementController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Admin;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class ManagementController : Controller
{

    private readonly IMediator _mediator;

    public ManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("admin/{organisationId}/events/created")]
    public async Task<IActionResult> RelaunchCreated([FromRoute] Guid organisationId)
    {
        await _mediator.Send(new RelaunchPictureEvents(organisationId, Topics.Pictures.Created));
        return Ok();
    }

    [HttpPost("admin/{organisationId}/events/updated")]
    public async Task<IActionResult> RelaunchUpdated([FromRoute] Guid organisationId)
    {
        await _mediator.Send(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated));
        return Ok();
    }

    [HttpPost("admin/{organisationId}/events/upload")]
    public async Task<IActionResult> RelaunchUpload([FromRoute] Guid organisationId)
    {
        await _mediator.Send(new RelaunchUpload(organisationId));
        return Ok();
    }
}