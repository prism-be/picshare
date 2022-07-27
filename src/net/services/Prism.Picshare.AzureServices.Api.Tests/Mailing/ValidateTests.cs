// -----------------------------------------------------------------------
//  <copyright file = "ValidateTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Prism.Picshare.AzureServices.Api.Mailing;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Mailing;

public class ValidateTests
{

    [Fact]
    public async Task Validate_Already_Consumed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionAlreadyConsumed);
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Validate(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object, id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Validate_NotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.MailActionNotFound);
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Validate(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object, id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Validate_Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultCodes.Ok);
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Validate(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object, id);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmationValidation>(r => r.Id == id), It.IsAny<CancellationToken>()), Times.Once);
    }
}