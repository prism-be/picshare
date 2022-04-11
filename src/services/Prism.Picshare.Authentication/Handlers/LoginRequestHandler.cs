// -----------------------------------------------------------------------
//  <copyright file="LoginRequestHandler.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Authentication.Commands;
using Prism.Picshare.Authentication.Model;
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
        _logger.LogDebug("Processing login for user {user} on {organisation}", request.Login, request.Organisation);

        using var db = _databaseResolver.GetDatabase(request.Organisation, DatabaseTypes.Authentication);

        var user = db.FindOne<User>(x => x.Login == request.Login);

        if (user == null)
        {
            user = new User( Guid.NewGuid(), request.Login, Argon2.Hash(request.Password), DateTime.UtcNow);
            db.Insert(user);

            return GenerateReponseOk(user);
        }

        if (Argon2.Verify(user.PasswordHash, request.Password))
        {
            return GenerateReponseOk(user);
        }

        return Task.FromResult(new LoginResponse(ReturnCodes.InvalidCredentials, null));
    }

    private Task<LoginResponse> GenerateReponseOk(User user)
    {
        throw new NotImplementedException();
    }
}