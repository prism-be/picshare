// -----------------------------------------------------------------------
//  <copyright file = "MailingController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Commands.Mailing;

namespace Prism.Picshare.Services.Api.Controllers;

public class MailingController : Controller
{
    private readonly IMediator _mediator;

    public MailingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("api/mailing/validate/{id:guid}")]
    public async Task<IActionResult> Validate([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new RegisterConfirmationValidation(id));

        switch (result)
        {
            case ResultCodes.Ok:
                return Ok(new
                {
                    code = ResultCodes.Ok
                });
            case ResultCodes.MailActionAlreadyConsumed:
            case ResultCodes.MailActionNotFound:
                return NotFound();
        }

        return BadRequest();
    }
}