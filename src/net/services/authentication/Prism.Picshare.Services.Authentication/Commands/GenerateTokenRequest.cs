// -----------------------------------------------------------------------
//  <copyright file = "GenerateTokenRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
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
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<GenerateTokenRequestHandler> _logger;
    private readonly StoreClient _storeClient;

    public GenerateTokenRequestHandler(ILogger<GenerateTokenRequestHandler> logger, JwtConfiguration jwtConfiguration, StoreClient storeClient)
    {
        _logger = logger;
        _jwtConfiguration = jwtConfiguration;
        _storeClient = storeClient;
    }

    public async Task<Token?> Handle(GenerateTokenRequest request, CancellationToken cancellationToken)
    {
        var credentials = await _storeClient.GetStateNullableAsync<Credentials>(request.Login, cancellationToken: cancellationToken);

        if (credentials == null)
        {
            _logger.LogWarning("No credentials found for login : {login}", request.Login);
            return null;
        }

        var key = EntityReference.ComputeKey(credentials.OrganisationId, credentials.Id);
        var user = await _storeClient.GetStateNullableAsync<User>(key, cancellationToken: cancellationToken);

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