// -----------------------------------------------------------------------
//  <copyright file = "PicshareAuthenticationHandler.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Prism.Picshare.Security;

namespace Prism.Picshare.AspNetCore.Authentication;

public class PicshareAuthenticationHandler : AuthenticationHandler<PicshareAuthenticationScheme>
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger _logger;

    public PicshareAuthenticationHandler(IOptionsMonitor<PicshareAuthenticationScheme> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
        JwtConfiguration jwtConfiguration) : base(options, logger, encoder, clock)
    {
        _jwtConfiguration = jwtConfiguration;
        _logger = logger.CreateLogger(typeof(PicshareAuthenticationHandler));
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Path.StartsWithSegments("/dapr", StringComparison.InvariantCultureIgnoreCase) || Request.Path.StartsWithSegments("/events"))
        {
            _logger.LogDebug("Allowing access to the path {path}", Request.Path);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new GenericIdentity("dapr")), "path")));
        }

        var bearer = GetBearerFromHeader();

        if (string.IsNullOrWhiteSpace(bearer))
        {
            bearer = GetBearerFromQuery();
        }

        if (string.IsNullOrWhiteSpace(bearer))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Header Not Found for path {Request.Path}"));
        }

        var claimsPrincipal = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, bearer, _logger, false);

        if (claimsPrincipal == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Bearer"));
        }

        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string GetBearerFromHeader()
    {
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return string.Empty;
        }

        var header = Request.Headers[HeaderNames.Authorization].ToString();

        return !header.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase)
            ? string.Empty
            : header[header.IndexOf(' ')..].Trim();
    }

    private string GetBearerFromQuery()
    {
        if (Request.Query.TryGetValue("accessToken", out var bearer))
        {
            return bearer;
        }

        return string.Empty;
    }
}