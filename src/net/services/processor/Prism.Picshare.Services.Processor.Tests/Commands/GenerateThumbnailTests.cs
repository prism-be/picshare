// -----------------------------------------------------------------------
//  <copyright file = "GenerateThumbnailTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Services.Processor.Commands;

namespace Prism.Picshare.Services.Processor.Tests.Commands;

public class GenerateThumbnailTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<GenerateThumbnailHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BindingResponse(new BindingRequest(Stores.Data, "get"), Samples.SmallImage, new Dictionary<string, string>()));

        // Act
        var handler = new GenerateThumbnailHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 25, 25), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
    }

    [Fact]
    public void Validate_Empty_Id()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.Empty, 42, 42);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Empty_Organisation()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.Empty, Guid.NewGuid(), 42, 42);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Negative_Height()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 42, 0);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Negative_Width()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), -1, 42);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 42, 42);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}