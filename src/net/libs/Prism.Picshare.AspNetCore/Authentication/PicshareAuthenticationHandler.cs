// -----------------------------------------------------------------------
//  <copyright file = "PicshareAuthenticationHandler.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Prism.Picshare.Events;
using Prism.Picshare.Exceptions;

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

        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            return Task.FromResult(AuthenticateResult.Fail($"Header Not Found for path {Request.Path}"));
        }

        var header = Request.Headers[HeaderNames.Authorization].ToString();

        if (!header.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Header Not Bearer"));
        }

        var bearer = header.Substring(header.IndexOf(' ')).Trim();

        var claimsPrincipal = ValidateToken(bearer);

        if (claimsPrincipal == null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Bearer"));
        }

        var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_jwtConfiguration.PublicKey))
            {
                throw new MissingConfigurationException("The configuration of jwtConfiguration.PublicKey is empty", "JWT_PUBLIC_KEY");
            }

            var publicKeyBytes = Convert.FromBase64String(_jwtConfiguration.PublicKey);

            using var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtConfiguration.Issuer,
                ValidAudience = JwtConfiguration.Audience,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                CryptoProviderFactory = new CryptoProviderFactory
                {
                    CacheSignatureProviders = false
                },
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occured when validating bearer");
            return null;
        }
    }
}