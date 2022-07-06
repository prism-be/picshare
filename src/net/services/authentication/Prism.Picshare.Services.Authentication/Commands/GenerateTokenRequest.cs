// -----------------------------------------------------------------------
//  <copyright file = "GenerateTokenRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Commands;

public record GenerateTokenRequest(string Login): IRequest<Token>;

public class GenerateTokenRequestValidator : AbstractValidator<GenerateTokenRequest>
{
    public GenerateTokenRequestValidator()
    {
    }
}

public class GenerateTokenRequestHandler : IRequestHandler<GenerateTokenRequest, Token>
{
    private readonly ILogger<GenerateTokenRequestHandler> _logger;
    private readonly DaprClient _daprClient;
    private readonly JwtConfiguration _jwtConfiguration;

    public GenerateTokenRequestHandler(ILogger<GenerateTokenRequestHandler> logger, DaprClient daprClient, JwtConfiguration jwtConfiguration)
    {
        _logger = logger;
        _daprClient = daprClient;
        _jwtConfiguration = jwtConfiguration;
    }

    public Task<Token> Handle(GenerateTokenRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}