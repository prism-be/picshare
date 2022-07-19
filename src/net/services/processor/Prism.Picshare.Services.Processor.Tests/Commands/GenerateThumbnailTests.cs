// -----------------------------------------------------------------------
//  <copyright file = "GenerateThumbnailTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

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
        var blobClient = new Mock<IBlobClient>();
        blobClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImage);

        // Act
        var handler = new GenerateThumbnailHandler(logger.Object, blobClient.Object);
        var result = await handler.Handle(new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 25, 25, false), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
    }

    [Fact]
    public async Task Handle_Ok_Crop()
    {
        // Arrange
        var logger = new Mock<ILogger<GenerateThumbnailHandler>>();
        var daprClient = new Mock<IBlobClient>();
        daprClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImage);

        // Act
        var handler = new GenerateThumbnailHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 25, 25, true), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
    }

    [Fact]
    public async Task Handle_Ok_Vertical()
    {
        // Arrange
        var logger = new Mock<ILogger<GenerateThumbnailHandler>>();
        var daprClient = new Mock<IBlobClient>();
        daprClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImageVertical);

        // Act
        var handler = new GenerateThumbnailHandler(logger.Object, daprClient.Object);
        var result = await handler.Handle(new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 25, 25, false), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
    }

    [Fact]
    public void Validate_Empty_Id()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.Empty, 42, 42, false);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Empty_Organisation()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.Empty, Guid.NewGuid(), 42, 42, false);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Negative_Height()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 42, 0, false);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Negative_Width()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), -1, 42, false);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new GenerateThumbnail(Guid.NewGuid(), Guid.NewGuid(), 42, 42, false);

        // Act
        var result = new GenerateThumbnailValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}