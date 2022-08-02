// -----------------------------------------------------------------------
//  <copyright file = "UserController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Api.Contracts;

namespace Prism.Picshare.Services.Api.Controllers;

public class UserController : Controller
{
    private readonly IUserContextAccessor _userContextAccessor;

    public UserController(IUserContextAccessor userContextAccessor)
    {
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [Route("api/authentication/user/check")]
    public IActionResult Check()
    {
        if (_userContextAccessor.IsAuthenticated)
        {
            return Ok(new UserAuthentication
            {
                Authenticated = _userContextAccessor.IsAuthenticated,
                Organisation = _userContextAccessor.OrganisationId,
                Name = _userContextAccessor.Name
            });
        }

        return Unauthorized(new UserAuthentication
        {
            Authenticated = false
        });
    }
}