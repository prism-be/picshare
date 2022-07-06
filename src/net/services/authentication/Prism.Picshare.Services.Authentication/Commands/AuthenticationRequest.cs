// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Commands;

public record AuthenticationRequest(string Login, string Password) : IRequest<ResponseCodes>;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    
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

    public Task<ResponseCodes> Handle(AuthenticationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}