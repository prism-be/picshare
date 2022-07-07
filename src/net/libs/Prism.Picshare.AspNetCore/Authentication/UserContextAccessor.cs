// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using Prism.Picshare.Domain;

namespace Prism.Picshare.AspNetCore.Authentication;

public class UserContextAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContextAccessor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public User GetCurrent()
    {
        return new User
        {
            Name = _contextAccessor.HttpContext.User?.Claims.SingleOrDefault(x => x.Type == "Name")?.Value ?? string.Empty,
        };
    }
}