// -----------------------------------------------------------------------
//  <copyright file = "GetPictureContentValidatorHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Services.Photobooth.Live.Commands;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Live.Tests;

public class GetPictureContentValidatorHandlerTests
{
    private static readonly Random Dice = new();

    [Fact]
    public async Task Handle_Empty()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        var logger = new Mock<ILogger<GetPictureContentValidatorHandler>>();
        var handler = new GetPictureContentValidatorHandler(daprClient.Object, logger.Object);

        // Act
        var result = await handler.Handle(new GetPictureContent(Guid.NewGuid(), Guid.NewGuid()), default);

        // Assert
        result.Should().Equal(Array.Empty<byte>());
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var content = new Byte[10];
        Dice.NextBytes(content);

        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), default))
            .ReturnsAsync(new BindingResponse(new BindingRequest(DaprConfiguration.DataStore, "get"), content, new Dictionary<string, string>()));
        var logger = new Mock<ILogger<GetPictureContentValidatorHandler>>();
        var handler = new GetPictureContentValidatorHandler(daprClient.Object, logger.Object);

        // Act
        var result = await handler.Handle(new GetPictureContent(Guid.NewGuid(), Guid.NewGuid()), default);

        // Assert
        result.Should().Equal(content);
    }
}