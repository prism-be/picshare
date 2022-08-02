// -----------------------------------------------------------------------
//  <copyright file = "MailingControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Services.Api.Controllers;

namespace Prism.Picshare.Services.Api.Tests.Controllers;

public class MailingControllerTests
{
    [Fact]
    public async Task Validate_Already_Consumed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionAlreadyConsumed);

        // Act
        var controller = new MailingController(mediator.Object);
        var result = await controller.Validate(id);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Validate_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionNotFound);

        // Act
        var controller = new MailingController(mediator.Object);
        var result = await controller.Validate(id);

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Validate_Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.Ok);

        // Act
        var controller = new MailingController(mediator.Object);
        var result = await controller.Validate(id);

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_Unknown()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.Unknown);

        // Act
        var controller = new MailingController(mediator.Object);
        var result = await controller.Validate(id);

        // Assert
        result.Should().BeAssignableTo<BadRequestResult>();
    }
}