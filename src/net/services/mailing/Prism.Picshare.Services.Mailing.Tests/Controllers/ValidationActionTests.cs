// -----------------------------------------------------------------------
//  <copyright file = "ValidationActionTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Controllers.Api;
using Xunit;

namespace Prism.Picshare.Services.Mailing.Tests.Controllers;

public class ValidationActionTests
{

    [Fact]
    public async Task Validate_Already_Consumed()
    {
        // Arrange
        var key = $"0-{Guid.NewGuid().ToString()}";
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionAlreadyConsumed);

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Validate_BadFormat()
    {
        // Arrange
        var key = "azerty";
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<BadRequestResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_NotFound()
    {
        // Arrange
        var key = $"0-{Guid.NewGuid().ToString()}";
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionNotFound);

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Validate_Null_Or_Empty()
    {
        // Arrange
        var key = string.Empty;
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<BadRequestResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_Ok()
    {
        // Arrange
        var key = $"0-{Guid.NewGuid().ToString()}";
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.Ok);

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_Unknown_Type()
    {
        // Arrange
        var key = $"666-{Guid.NewGuid().ToString()}";
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new ValidationAction(mediator.Object);
        var result = await controller.Validate(key);

        // Assert
        result.Should().BeAssignableTo<BadRequestResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Key == key), It.IsAny<CancellationToken>()), Times.Never);
    }
}