// -----------------------------------------------------------------------
//  <copyright file="LoginRequestHandler.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Prism.Picshare.Authentication.Commands;
using Prism.Picshare.Authentication.Model;
using Prism.Picshare.Data;
using Prism.Picshare.Events;
using Prism.Picshare.Events.Model;
using Prism.Picshare.Security;

namespace Prism.Picshare.Authentication.Handlers;

public class LoginRequestHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IDatabaseResolver _databaseResolver;
    private readonly IEventPublisher _eventPublisher;
    private readonly JwtConfiguration _jwtConfiguration;
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
            user = new User(Guid.NewGuid(), request.Login, Argon2.Hash(request.Password), DateTime.UtcNow);
            db.Insert(user);

            _eventPublisher.Publish(Topics.Authentication.UserAuthenticated, new UserAuthenticated(user.Id, ReturnCodes.Ok));
            return Task.FromResult(GenerateReponseOk(user));
        }

        if (Argon2.Verify(user.PasswordHash, request.Password))
        {
            _eventPublisher.Publish(Topics.Authentication.UserAuthenticated, new UserAuthenticated(user.Id, ReturnCodes.Ok));
            return Task.FromResult(GenerateReponseOk(user));
        }

        return Task.FromResult(new LoginResponse(ReturnCodes.InvalidCredentials, null));
    }

    private string GenerateJwt(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //claim is used to add identity to JWT token
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()), new Claim(JwtRegisteredClaimNames.Name, user.Login), new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };

        var token = new JwtSecurityToken(_jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private LoginResponse GenerateReponseOk(User user)
    {
        return new LoginResponse(ReturnCodes.Ok, GenerateJwt(user));
    }
}