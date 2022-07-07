// -----------------------------------------------------------------------
//  <copyright file = "LoginController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Authentication.Commands;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class LoginController : Controller
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/api/authentication/login")]
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

    [HttpPost("/api/authentication/register")]
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