// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace Prism.Picshare.AspNetCore.Authentication;

public interface IUserContextAccessor
{
    bool IsAuthenticated { get; }
    Guid Id { get; }
    Guid OrganisationId { get; }
    string Name { get; }

    bool HasAccess(Guid organisationId);
}

public class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContextAccessor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool IsAuthenticated => _contextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public Guid Id => Guid.Parse(_contextAccessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == "Id")?.Value ?? Guid.Empty.ToString());

    public Guid OrganisationId => Guid.Parse(_contextAccessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == "OrganisationId")?.Value ?? Guid.Empty.ToString());

    public string Name => _contextAccessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == "Name")?.Value ?? string.Empty;

    public bool HasAccess(Guid organisationId)
    {
        return OrganisationId == organisationId;
    }
}