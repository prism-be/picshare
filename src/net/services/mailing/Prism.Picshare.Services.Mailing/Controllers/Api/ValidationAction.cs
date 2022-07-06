// -----------------------------------------------------------------------
//  <copyright file = "ValidationAction.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Model;

namespace Prism.Picshare.Services.Mailing.Controllers.Api;

public class ValidationAction : Controller
{
    private readonly IMediator _mediator;

    public ValidationAction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("api/mailing/validate/{key}")]
    public async Task<IActionResult> Validate([FromRoute] string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return BadRequest();
        }

        var result = ResultCodes.Unknown;

        var keyPart = key.Split('-')[0];

        if (!Int32.TryParse(keyPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var type))
        {
            return BadRequest();
        }

        switch (type)
        {
            case (int)MailActionType.ConfirmUserRegistration:
                result = await _mediator.Send(new RegisterConfirmationValidation(key));
                break;
        }

        switch (result)
        {
            case ResultCodes.Ok:
                return Ok();
            case ResultCodes.MailActionAlreadyConsumed:
            case ResultCodes.MailActionNotFound:
                return NotFound();
        }

        return BadRequest();
    }
}