// -----------------------------------------------------------------------
//  <copyright file = "UserController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Authentication.Commands;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class UserController : Controller
{
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IMediator _mediator;

    public UserController(IUserContextAccessor userContextAccessor, IMediator mediator)
    {
        _userContextAccessor = userContextAccessor;
        _mediator = mediator;
    }

    [HttpGet("/api/authentication/user/check")]
    public IActionResult Check()
    {
        if (!_userContextAccessor.IsAuthenticated)
        {
            return Unauthorized(new
            {
                authenticated = _userContextAccessor.IsAuthenticated,
            });
        }

        return Ok(new
        {
            authenticated = _userContextAccessor.IsAuthenticated,
            name = _userContextAccessor.Name
        });
    }
}