// -----------------------------------------------------------------------
//  <copyright file = "SubscribeRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Authentication;

public class RegisterAccountRequestTests
{

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var storeClient = new Mock<StoreClient>();
        var publisherClient = new Mock<PublisherClient>();

        // Act
        var handler = new RegisterAccountRequestHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RegisterAccountRequest(login, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
            CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        storeClient.VerifySaveState<Organisation>(Stores.Organisations);
        storeClient.VerifySaveState<User>(Stores.Users);
        publisherClient.VerifyPublishEvent<User>(Topics.User.Register);
    }

    [Fact]
    public async Task Handle_User_Exist()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Credentials, string.Empty, login, new Credentials());

        // Act
        var handler = new RegisterAccountRequestHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RegisterAccountRequest(login, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()),
            CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.ExistingUsername);
    }

    [Fact]
    public void Validator_Invalid_Email()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Missing_Email()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Missing_Login()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Missing_Name()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Missing_Organisation()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), string.Empty));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Missing_Password()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), string.Empty, Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Ok()
    {
        // Arrange
        var validator = new RegisterAccountRequestValidator();

        // Act
        var result = validator.Validate(new RegisterAccountRequest(Guid.NewGuid().ToString(), "hello@pichsare.me", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()));

        // Assert
        result.IsValid.Should().BeTrue();
    }
}