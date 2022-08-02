// -----------------------------------------------------------------------
//  <copyright file = "AdminController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Commands.Pictures.Admin;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Api.Controllers;

public class AdminController : Controller
{
    private readonly IMediator _mediator;
    private readonly IUserContextAccessor _userContextAccessor;

    public AdminController(IMediator mediator, IUserContextAccessor userContextAccessor)
    {
        _mediator = mediator;
        _userContextAccessor = userContextAccessor;
    }

    [HttpPost]
    [Route("api/admin/events/created")]
    public async Task<IActionResult> EventsCreated()
    {
        await _mediator.Send(new RelaunchPictureEvents(_userContextAccessor.OrganisationId, Topics.Pictures.Created));
        return NoContent();
    }

    [HttpPost]
    [Route("api/admin/events/updated")]
    public async Task<IActionResult> EventsUpdated()
    {
        await _mediator.Send(new RelaunchPictureEvents(_userContextAccessor.OrganisationId, Topics.Pictures.Updated));
        return NoContent();
    }

    [HttpPost]
    [Route("api/admin/events/uploaded")]
    public async Task<IActionResult> EventsUploaded()
    {
        await _mediator.Send(new RelaunchUpload(_userContextAccessor.OrganisationId));
        return NoContent();
    }
}