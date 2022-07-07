// -----------------------------------------------------------------------
//  <copyright file = "GenerateTokenRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Dapr.Client;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Authentication.Commands;

public record GenerateTokenRequest(string Login) : IRequest<Token?>;

public class GenerateTokenRequestValidator : AbstractValidator<GenerateTokenRequest>
{
    public GenerateTokenRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class GenerateTokenRequestHandler : IRequestHandler<GenerateTokenRequest, Token?>
{
    private readonly DaprClient _daprClient;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<GenerateTokenRequestHandler> _logger;

    public GenerateTokenRequestHandler(ILogger<GenerateTokenRequestHandler> logger, DaprClient daprClient, JwtConfiguration jwtConfiguration)
    {
        _logger = logger;
        _daprClient = daprClient;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<Token?> Handle(GenerateTokenRequest request, CancellationToken cancellationToken)
    {
        var credentials = await _daprClient.GetStateAsync<Credentials>(Stores.Credentials, request.Login, cancellationToken: cancellationToken);

        if (credentials == null)
        {
            _logger.LogWarning("No credentials found for login : {login}", request.Login);
            return null;
        }

        var user = await _daprClient.GetStateAsync<User>(Stores.Users, credentials.Key, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("No user found for key : {key}", credentials.Key);
            return null;
        }

        var token = new Token
        {
            AccessToken = GenerateAccessToken(user),
            RefreshToken = GenerateRefreshToken(user),
            Expires = (int)TimeSpan.FromMinutes(30).TotalSeconds
        };

        return token;
    }

    private string GenerateAccessToken(User user)
    {
        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Security.GenerateIdentifier().ToString()),
            new(nameof(user.Name), user.Name),
            new(nameof(user.OrganisationId), user.OrganisationId.ToString()),
            new(nameof(user.Id), user.Id.ToString())
        };

        return GenerateToken(claims, DateTime.Now.AddMinutes(30));
    }

    private string GenerateRefreshToken(User user)
    {
        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Security.GenerateIdentifier().ToString()),
            new(nameof(user.Key), user.Key)
        };

        return GenerateToken(claims, DateTime.Now.AddDays(30));
    }

    private string GenerateToken(IEnumerable<Claim> claims, DateTime expiration)
    {
        var privateKey = Convert.FromBase64String(_jwtConfiguration.PrivateKey);

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        var now = DateTime.Now;

        var jwt = new JwtSecurityToken(
            audience: JwtConfiguration.Audience,
            issuer: JwtConfiguration.Issuer,
            claims: claims,
            notBefore: now,
            expires: expiration,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}