// -----------------------------------------------------------------------
//  <copyright file = "RefreshTokenRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Security;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Authentication;

public record RefreshTokenRequest(string RefreshToken) : IRequest<Token?>
{
    public override string? ToString()
    {
        return base.ToString()?.Replace(RefreshToken, "***");
    }
}

public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Token?>
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<GenerateTokenRequestHandler> _logger;
    private readonly StoreClient _storeClient;

    public RefreshTokenRequestHandler(JwtConfiguration jwtConfiguration, ILogger<GenerateTokenRequestHandler> logger, StoreClient storeClient)
    {
        _jwtConfiguration = jwtConfiguration;
        _logger = logger;
        _storeClient = storeClient;
    }

    public async Task<Token?> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, request.RefreshToken, _logger, true);

        if (principal == null)
        {
            return null;
        }

        var id = Guid.Parse(principal.Claims.SingleOrDefault(x => x.Type == "Id")?.Value ?? Guid.Empty.ToString());
        var organisationId = Guid.Parse(principal.Claims.SingleOrDefault(x => x.Type == "OrganisationId")?.Value ?? Guid.Empty.ToString());
        var user = await _storeClient.GetStateNullableAsync<User>(organisationId, id, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("No user found for key : {key}", id);
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