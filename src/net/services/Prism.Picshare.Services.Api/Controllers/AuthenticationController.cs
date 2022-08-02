// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Commands.Authentication;

namespace Prism.Picshare.Services.Api.Controllers;

public class AuthenticationController : Controller
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("api/authentication/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
    {
        var result = await _mediator.Send(request);

        if (result == ResultCodes.Ok)
        {
            var token = await _mediator.Send(new GenerateTokenRequest(request.Login));

            if (token != null)
            {
                return Ok(token);
            }

            return BadRequest();
        }

        return Unauthorized();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("api/authentication/refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var token = await _mediator.Send(request);

        if (token != null)
        {
            return Ok(token);
        }

        return Unauthorized();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("api/authentication/register")]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {
        var responseCode = await _mediator.Send(request);

        if (responseCode == ResultCodes.Ok)
        {
            return NoContent();
        }

        return Conflict(new
        {
            code = responseCode
        });
    }
}