﻿// -----------------------------------------------------------------------
//  <copyright file = "LoginController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [AllowAnonymous]
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

    [AllowAnonymous]
    [HttpPost("/api/authentication/refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var token = await _mediator.Send(request);

        if (token != null)
        {
            return Ok(token);
        }

        return Unauthorized();
    }
    
}