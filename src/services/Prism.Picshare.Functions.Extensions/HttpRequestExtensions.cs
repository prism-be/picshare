// -----------------------------------------------------------------------
//  <copyright file="HttpRequestExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace Prism.Picshare.Functions.Extensions;

public static class HttpRequestExtensions
{
    public static Guid GetOrganisationId(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue("X-OrganisationId", out var header))
        {
            throw new UnauthorizedAccessException("The organisation id cannot be found");
        }

        if (!Guid.TryParse(header, out var organisationId))
        {
            throw new UnauthorizedAccessException("The organisation id is not well formatted");
        }

        return organisationId;
    }
}