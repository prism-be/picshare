// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationValidationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Mailing;

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
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.MailActions, string.Empty, action.Id.ToString(), action);

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Id), CancellationToken.None);

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
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Id), CancellationToken.None);

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
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.MailActions, string.Empty, action.Id.ToString(), action);

        // Act
        var handler = new RegisterConfirmationValidationHandler(Mock.Of<ILogger<RegisterConfirmationValidationHandler>>(), storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RegisterConfirmationValidation(action.Id), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        publisherClient.VerifyPublishEvent<User>(Topics.Email.Validated);
        storeClient.VerifySaveState<MailAction<User>>(Stores.MailActions);
    }

    [Fact]
    public void Validation_Empty_Key()
    {
        // Arrange
        var request = new RegisterConfirmationValidation(Guid.Empty);

        // Act
        var result = new RegisterConfirmationValidationValidation().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validation_Ok()
    {
        // Arrange
        var request = new RegisterConfirmationValidation(Guid.NewGuid());

        // Act
        var result = new RegisterConfirmationValidationValidation().Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}