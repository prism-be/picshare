// -----------------------------------------------------------------------
//  <copyright file = "EmailValidatedTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Prism.Picshare.AzureServices.Workers.User;
using Prism.Picshare.Commands.Authentication;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.User;

public class EmailValidatedTests
{

    [Fact]
    public async Task Validate_NotFound()
    {
        // Arrange
        var user = new Domain.User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<EmailValidatedRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(ResultCodes.UserNotFound);

        var message = JsonSerializer.Serialize(user);
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new EmailValidated(mediator.Object);
        var result = await controller.Run(message, context.Object);

        // Assert
        result.Should().Be(ResultCodes.MailActionNotFound);
    }

    [Fact]
    public async Task Validate_Ok()
    {
        // Arrange
        var user = new Domain.User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<EmailValidatedRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(ResultCodes.Ok);

        var message = JsonSerializer.Serialize(user);
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new EmailValidated(mediator.Object);
        var result = await controller.Run(message, context.Object);

        // Assert
        result.Should().Be(ResultCodes.Ok);
    }
}