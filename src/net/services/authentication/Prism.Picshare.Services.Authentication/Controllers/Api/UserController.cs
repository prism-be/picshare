// -----------------------------------------------------------------------
//  <copyright file = "UserController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class UserController : Controller
{
    [HttpGet("/api/authentication/user/check")]
    public async Task<IActionResult> Check()
    {
        return Ok(this.User.Identity?.Name);
    }
}