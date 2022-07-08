// -----------------------------------------------------------------------
//  <copyright file = "UserController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Authentication.Contracts;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class UserController : Controller
{
    private readonly IUserContextAccessor _userContextAccessor;

    public UserController(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet("/api/authentication/user/check")]
    public IActionResult Check()
    {
        if (!_userContextAccessor.IsAuthenticated)
        {
            return Ok(new UserAuthentication
            {
                Authenticated = false
            });
        }

        return Ok(new UserAuthentication
        {
            Authenticated = _userContextAccessor.IsAuthenticated,
            Name = _userContextAccessor.Name
        });
    }
}