// -----------------------------------------------------------------------
//  <copyright file="LoginRequestHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Authentication.Commands;
using Prism.Picshare.Authentication.Handlers;
using Prism.Picshare.Data;
using Prism.Picshare.Events;
using Xunit;

namespace Prism.Picshare.Authentication.Tests.Handlers;

public class LoginRequestHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisation = "tests";
        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var jwtConfiguration = new JwtConfiguration();
        var databaseResolver = new Mock<IDatabaseResolver>();
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result1 = await handler.Handle(new LoginRequest(organisation, user, password), default);
        var result2 = await handler.Handle(new LoginRequest(organisation, user, password), default);

        // Assert
        Assert.Equal(ReturnCodes.Ok, result1.ReturnCode);
        Assert.NotNull(result1.Token);
        Assert.Equal(ReturnCodes.Ok, result2.ReturnCode);
        Assert.NotNull(result2.Token);
    }

    [Fact]
    public async Task Handle_Wrong_Password()
    {
        // Arrange
        var organisation = "tests";
        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var jwtConfiguration = new JwtConfiguration();
        var databaseResolver = new Mock<IDatabaseResolver>();
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result1 = await handler.Handle(new LoginRequest(organisation, user, password), default);
        var result2 = await handler.Handle(new LoginRequest(organisation, user, "42"), default);

        // Assert
        Assert.Equal(ReturnCodes.Ok, result1.ReturnCode);
        Assert.NotNull(result1.Token);
        Assert.Equal(ReturnCodes.InvalidCredentials, result2.ReturnCode);
        Assert.Null(result2.Token);
    }

    [Fact]
    public async Task Handle_Wrong_Username()
    {
        // Arrange
        var organisation = "tests";
        var user = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();

        var jwtConfiguration = new JwtConfiguration();
        var databaseResolver = new Mock<IDatabaseResolver>();
        var eventPublisher = new Mock<IEventPublisher>();
        var logger = new Mock<ILogger<LoginRequestHandler>>();

        var handler = new LoginRequestHandler(jwtConfiguration, databaseResolver.Object, eventPublisher.Object, logger.Object);

        // Act
        var result1 = await handler.Handle(new LoginRequest(organisation, user, password), default);
        var result2 = await handler.Handle(new LoginRequest(organisation, "42", password), default);

        // Assert
        Assert.Equal(ReturnCodes.Ok, result1.ReturnCode);
        Assert.NotNull(result1.Token);
        Assert.Equal(ReturnCodes.InvalidCredentials, result2.ReturnCode);
        Assert.Null(result2.Token);
    }
}