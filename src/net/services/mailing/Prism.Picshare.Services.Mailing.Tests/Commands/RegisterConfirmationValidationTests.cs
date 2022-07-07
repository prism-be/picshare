// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationValidationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Acme.Dapr.Extensions.UnitTesting;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Model;
using Xunit;
using Stores = Prism.Picshare.Services.Mailing.Model.Stores;

namespace Prism.Picshare.Services.Mailing.Tests.Commands;

public class RegisterConfirmationValidationTests
{

    [Fact]
    public async Task Handler_Already_Validated()
    {
        // Arrange
        var key = Guid.NewGuid();
        var user = new User();
        var action = new MailAction<User>(key, MailActionType.ConfirmUserRegistration, user)
        {
            Consumed = true
        };
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.MailActions, action.Key, action);

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), daprClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Key), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.MailActionAlreadyConsumed);
    }

    [Fact]
    public async Task Handler_NotFound()
    {
        // Arrange
        var key = Guid.NewGuid();
        var user = new User();
        var action = new MailAction<User>(key, MailActionType.ConfirmUserRegistration, user);
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), daprClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Key), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.MailActionNotFound);
    }

    [Fact]
    public async Task Handler_Ok()
    {
        // Arrange
        var key = Guid.NewGuid();
        var user = new User();
        var action = new MailAction<User>(key, MailActionType.ConfirmUserRegistration, user);
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.MailActions, action.Key, action);

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), daprClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Key), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        daprClient.VerifyPublishEvent<User>(Publishers.PubSub, Topics.Email.Validated);
        daprClient.VerifySaveState<MailAction<User>>(Stores.MailActions);
    }

    [Fact]
    public void Validation_Empty_Key()
    {
        // Arrange
        var request = new RegisterConfirmationValidation(string.Empty);

        // Act
        var result = new RegisterConfirmationValidationValidation().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validation_Ok()
    {
        // Arrange
        var request = new RegisterConfirmationValidation(Guid.NewGuid().ToString());

        // Act
        var result = new RegisterConfirmationValidationValidation().Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}