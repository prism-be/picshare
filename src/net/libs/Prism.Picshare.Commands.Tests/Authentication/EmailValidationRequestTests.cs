// -----------------------------------------------------------------------
//  <copyright file = "EmailValidationRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Authentication;

public class EmailValidationRequestTests
{
    [Fact]
    public async Task Handle_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailValidatedRequestHandler>>();
        var storeClient = new Mock<StoreClient>();

        var request = new EmailValidatedRequest(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var handler = new EmailValidatedRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.UserNotFound);
    }
    
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailValidatedRequestHandler>>();
        var storeClient = new Mock<StoreClient>();

        var request = new EmailValidatedRequest(Guid.NewGuid(), Guid.NewGuid());
        storeClient.SetupGetStateAsync(Stores.Users, request.OrganisationId, request.UserId, new User
        {
            OrganisationId = request.OrganisationId,
            Id = request.UserId
        });

        // Act
        var handler = new EmailValidatedRequestHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        storeClient.VerifySaveState<User>(Stores.Users);
    }

    [Fact]
    public async Task Validator_Empty_Id()
    {
        // Arrange
        var request = new EmailValidatedRequest(Guid.NewGuid(), Guid.Empty);

        // Act
        var validator = new EmailValidatedRequestValidator();
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validator_Empty_Organisation()
    {
        // Arrange
        var request = new EmailValidatedRequest(Guid.Empty, Guid.NewGuid());

        // Act
        var validator = new EmailValidatedRequestValidator();
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validator_Ok()
    {
        // Arrange
        var request = new EmailValidatedRequest(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var validator = new EmailValidatedRequestValidator();
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}