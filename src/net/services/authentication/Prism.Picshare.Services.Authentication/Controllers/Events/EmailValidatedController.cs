// -----------------------------------------------------------------------
//  <copyright file = "EmailValidatedController.cs" company = "Prism">
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
using Prism.Picshare.Services.Authentication.Commands;

namespace Prism.Picshare.Services.Authentication.Controllers.Events;

[AllowAnonymous]
public class EmailValidatedController : Controller
{
    private readonly IMediator _mediator;

    public EmailValidatedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Topic(Publishers.PubSub, Topics.Email.Validated)]
    [HttpPost(Topics.RoutePrefix + Topics.Email.Validated)]
    public async Task<IActionResult> Validate(User user)
    {
        var result = await _mediator.Send(new EmailValidatedRequest(user.OrganisationId, user.Id));

        return result == ResultCodes.Ok ? Ok() : NotFound();
    }
}