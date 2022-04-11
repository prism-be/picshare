// -----------------------------------------------------------------------
//  <copyright file="LoginRequestHandler.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Authentication.Commands;
using Prism.Picshare.Data;
using Prism.Picshare.Events;

namespace Prism.Picshare.Authentication.Handlers;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly IDatabaseResolver _databaseResolver;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<LoginRequestHandler> _logger;

    public LoginRequestHandler(JwtConfiguration jwtConfiguration, IDatabaseResolver databaseResolver, IEventPublisher eventPublisher, ILogger<LoginRequestHandler> logger)
    {
        _jwtConfiguration = jwtConfiguration;
        _databaseResolver = databaseResolver;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}