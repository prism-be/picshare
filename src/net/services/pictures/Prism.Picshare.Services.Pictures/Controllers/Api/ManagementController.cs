// -----------------------------------------------------------------------
//  <copyright file = "ManagementController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Admin;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class ManagementController : Controller
{
    private readonly IMediator _mediator;
    private readonly IUserContextAccessor _userContextAccessor;

    public ManagementController(IMediator mediator, IUserContextAccessor userContextAccessor)
    {
        _mediator = mediator;
        _userContextAccessor = userContextAccessor;
    }

    [HttpPost("api/pictures/admin/events/created")]
    public async Task<IActionResult> RelaunchCreated()
    {
        await _mediator.Send(new RelaunchPictureEvents(_userContextAccessor.OrganisationId, Topics.Pictures.Created));
        return Ok();
    }

    [HttpPost("api/pictures/admin/events/updated")]
    public async Task<IActionResult> RelaunchUpdated()
    {
        await _mediator.Send(new RelaunchPictureEvents(_userContextAccessor.OrganisationId, Topics.Pictures.Updated));
        return Ok();
    }

    [HttpPost("api/pictures/admin/upload")]
    public async Task<IActionResult> RelaunchUpload()
    {
        await _mediator.Send(new RelaunchUpload(_userContextAccessor.OrganisationId));
        return Ok();
    }
}