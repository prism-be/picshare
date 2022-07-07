// -----------------------------------------------------------------------
//  <copyright file = "UserController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class UserController : Controller
{
    [HttpGet("/api/authentication/user/check")]
    public IActionResult Check()
    {
        return Ok(new
        {
            authenticated = User.Identity?.IsAuthenticated,
            name = User.Claims.SingleOrDefault(x => x.Type == "Name")?.Value
        });
    }
}