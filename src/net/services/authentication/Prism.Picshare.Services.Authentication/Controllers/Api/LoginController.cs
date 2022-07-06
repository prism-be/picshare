// -----------------------------------------------------------------------
//  <copyright file = "LoginController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Authentication.Commands;
using Prism.Picshare.Services.Authentication.Configuration;

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
        throw new NotImplementedException();
    }

    [HttpPost("/api/authentication/register")]
    public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
    {
        var responseCode = await _mediator.Send(request);

        if (responseCode == ResponseCodes.Ok)
        {
            return NoContent();
        }

        return Conflict(new
        {
            code = responseCode
        });
    }
}