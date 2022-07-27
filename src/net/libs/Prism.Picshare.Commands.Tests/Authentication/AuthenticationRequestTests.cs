// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Authentication;

public class AuthenticationRequestTests
{

    [Fact]
    public async Task Handle_Email_Not_Validated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(login, password);

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Credentials, string.Empty, login, new Credentials
        {
            UserId = userId,
            OrganisationId = organisationId,
            Id = login,
            PasswordHash = passwordHash
        });

        storeClient.SetupGetStateAsync(Stores.Users, organisationId, userId, new User
        {
            Id = userId,
            OrganisationId = organisationId,
            EmailValidated = false
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.EmailNotValidated);
    }

    [Fact]
    public async Task Handle_Invalid_Login()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Credentials, string.Empty, login, new Credentials
        {
            UserId = Guid.NewGuid(),
            Id = login,
            PasswordHash = passwordHash
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.InvalidCredentials);
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
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Credentials, string.Empty, login, new Credentials
        {
            UserId = Guid.NewGuid(),
            Id = login,
            PasswordHash = passwordHash
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var login = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        var passwordHash = Argon2.Hash(password, 1, 10);

        var request = new AuthenticationRequest(login, password);

        var logger = new Mock<ILogger<AuthenticationRequestHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Credentials, string.Empty, login, new Credentials
        {
            UserId = userId,
            OrganisationId = organisationId,
            Id = login,
            PasswordHash = passwordHash
        });

        storeClient.SetupGetStateAsync(Stores.Users, organisationId, userId, new User
        {
            Id = userId,
            OrganisationId = organisationId,
            EmailValidated = true
        });

        // Act
        var handler = new AuthenticationRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
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