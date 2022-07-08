// -----------------------------------------------------------------------
//  <copyright file = "RefreshTokenRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Authentication.Commands;

public record RefreshTokenRequest(string RefreshToken) : IRequest<Token?>
{
    public override string? ToString()
    {
        return base.ToString()?.Replace(RefreshToken, "***");
    }
}

public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Token?>
{
    private readonly DaprClient _daprClient;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<GenerateTokenRequestHandler> _logger;

    public RefreshTokenRequestHandler(DaprClient daprClient, JwtConfiguration jwtConfiguration, ILogger<GenerateTokenRequestHandler> logger)
    {
        _daprClient = daprClient;
        _jwtConfiguration = jwtConfiguration;
        _logger = logger;
    }

    public async Task<Token?> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, request.RefreshToken, _logger, true);

        if (principal == null)
        {
            return null;
        }

        var key = principal.Claims.SingleOrDefault(x => x.Type == "Key")?.Value ?? Guid.Empty.ToString();
        var user = await _daprClient.GetStateAsync<User>(Stores.Users, key, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("No user found for key : {key}", key);
            return null;
        }

        var token = new Token
        {
            AccessToken = TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user),
            RefreshToken = TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user),
            Expires = (int)TimeSpan.FromMinutes(30).TotalSeconds
        };

        return token;
    }
}