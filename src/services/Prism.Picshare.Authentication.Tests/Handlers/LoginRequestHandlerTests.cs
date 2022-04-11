// -----------------------------------------------------------------------
//  <copyright file="LoginRequestHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Authentication.Commands;
using Prism.Picshare.Authentication.Handlers;
using Prism.Picshare.Authentication.Model;
using Prism.Picshare.Data;
using Prism.Picshare.Events;
using Xunit;

namespace Prism.Picshare.Authentication.Tests.Handlers;

public class LoginRequestHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        var organisationId = "tests";
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var user = new User(Guid.NewGuid(), login, Argon2.Hash(password, 1, 1024), DateTime.UtcNow);

        var db = new Mock<IDatabase>();
        db.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);

        var databaseResolver = new Mock<IDatabaseResolver>();
        databaseResolver.Setup(x => x.GetDatabase(organisationId, DatabaseTypes.Authentication)).Returns(db.Object);

        var jwtConfiguration = new JwtConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result = await handler.Handle(new LoginRequest(organisationId, login, password), default);

        // Assert
        Assert.Equal(ReturnCodes.Ok, result.ReturnCode);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Handle_Wrong_Password()
    {
        // Arrange
        var organisationId = "tests";
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var user = new User(Guid.NewGuid(), login, Argon2.Hash(password, 1, 1024), DateTime.UtcNow);

        var db = new Mock<IDatabase>();
        db.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(user);

        var databaseResolver = new Mock<IDatabaseResolver>();
        databaseResolver.Setup(x => x.GetDatabase(organisationId, DatabaseTypes.Authentication)).Returns(db.Object);

        var jwtConfiguration = new JwtConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result = await handler.Handle(new LoginRequest(organisationId, login, "42"), default);

        // Assert
        Assert.Equal(ReturnCodes.InvalidCredentials, result.ReturnCode);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task Handle_Wrong_Username()
    {
        var organisationId = "tests";
        var password = Guid.NewGuid().ToString();

        var db = new Mock<IDatabase>();

        var databaseResolver = new Mock<IDatabaseResolver>();
        databaseResolver.Setup(x => x.GetDatabase(organisationId, DatabaseTypes.Authentication)).Returns(db.Object);

        var jwtConfiguration = new JwtConfiguration(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result = await handler.Handle(new LoginRequest(organisationId, "42", password), default);

        // Assert
        Assert.Equal(ReturnCodes.Ok, result.ReturnCode);
        Assert.NotNull(result.Token);
    }
}