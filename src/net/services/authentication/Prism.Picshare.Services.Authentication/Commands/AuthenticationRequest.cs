// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Commands;

public record AuthenticationRequest(string Login, string Password) : IRequest<ResponseCodes>;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class AuthenticationRequestHandler : IRequestHandler<AuthenticationRequest, ResponseCodes>
{
    private readonly ILogger<AuthenticationRequestHandler> _logger;
    private readonly DaprClient _daprClient;

    public AuthenticationRequestHandler(ILogger<AuthenticationRequestHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<ResponseCodes> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var credentials = await _daprClient.GetStateAsync<Credentials>(Stores.Credentials, request.Login, cancellationToken: cancellationToken);

        if (credentials == null)
        {
            _logger.LogInformation("Credentials not found for login : {login}", request.Login);
            return ResponseCodes.InvalidCredentials;
        }

        if (Argon2.Verify(credentials.PasswordHash, request.Password))
        {
            _logger.LogInformation("Authentication success for credentials : {id}", credentials.Id);
            return ResponseCodes.Ok;
        }
        
        _logger.LogInformation("Authentication failed for credentials : {id}", credentials.Id);
        return ResponseCodes.InvalidCredentials;
    }
}