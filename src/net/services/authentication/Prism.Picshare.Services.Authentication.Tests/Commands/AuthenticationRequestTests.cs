// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentAssertions;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Commands;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Tests.Commands;

public class AuthenticationRequestTests
{

    [Fact]
    public async Task Handle_Invalid_Login()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Credentials, login, new Credentials
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResponseCodes.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Invalid_Password()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(login, Guid.NewGuid().ToString());

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Credentials, login, new Credentials
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResponseCodes.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(login, password);

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Credentials, login, new Credentials
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = passwordHash
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResponseCodes.Ok);
    }

    [Fact]
    public void Validate_Empty_Login()
    {
        // Arrange
        var request = new AuthenticationRequest(string.Empty, Guid.NewGuid().ToString());

        // Act
        var validator = new AuthenticationRequestValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Empty_Password()
    {
        // Arrange
        var request = new AuthenticationRequest(Guid.NewGuid().ToString(), string.Empty);

        // Act
        var validator = new AuthenticationRequestValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        // Act
        var validator = new AuthenticationRequestValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}