// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Authentication;

public record AuthenticationRequest(string Login, string Password) : IRequest<ResultCodes>
{
    public override string? ToString()
    {
        return base.ToString()?.Replace(Password, string.Empty.PadRight(Password.Length, '*'));
    }
}

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class AuthenticationRequestHandler : IRequestHandler<AuthenticationRequest, ResultCodes>
{
    private readonly ILogger<AuthenticationRequestHandler> _logger;
    private readonly StoreClient _storeClient;

    public AuthenticationRequestHandler(ILogger<AuthenticationRequestHandler> logger, StoreClient storeClient)
    {
        _logger = logger;
        _storeClient = storeClient;
    }

    public async Task<ResultCodes> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        var credentials = await _storeClient.GetStateNullableAsync<Credentials>(request.Login, cancellationToken: cancellationToken);
        
        if (credentials == null)
        {
            _logger.LogInformation("Credentials not found for login : {login}", request.Login);
            return ResultCodes.InvalidCredentials;
        }

        if (Argon2.Verify(credentials.PasswordHash, request.Password))
        {
            var user = await _storeClient.GetStateNullableAsync<User>(credentials.OrganisationId, credentials.UserId, cancellationToken: cancellationToken);
            
            if (user == null)
            {
                _logger.LogInformation("Credentials not found for login : {login}", request.Login);
                return ResultCodes.UserNotFound;
            }

            if (user.EmailValidated)
            {
                _logger.LogInformation("Authentication success for credentials : {id}", credentials.UserId);
                return ResultCodes.Ok;
            }
            
            _logger.LogInformation("Authentication failed for credentials, email not validated : {id}", credentials.UserId);
            return ResultCodes.EmailNotValidated;
        }
        
        _logger.LogInformation("Authentication failed for credentials : {id}", credentials.UserId);
        return ResultCodes.InvalidCredentials;
    }
}