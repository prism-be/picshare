// -----------------------------------------------------------------------
//  <copyright file = "ReadMetaDataTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentAssertions;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Processor.Commands;
using Prism.Picshare.UnitTests;

namespace Prism.Picshare.Services.Processor.Tests.Commands;

public class ReadMetaDataTests
{

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BindingResponse(new BindingRequest(Stores.Data, "get"), Samples.SmallImage, new Dictionary<string, string>()));

        // Act
        var handler = new ReadMetaDataHandler(daprClient.Object);
        var result = await handler.Handle(new ReadMetaData(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        daprClient.VerifyPublishEvent<Picture>(Publishers.PubSub, Topics.Pictures.ExifRead);
    }

    [Fact]
    public void Validate_Ko_Null()
    {
        // Arrange
        var request = new ReadMetaData(Guid.Empty, Guid.NewGuid());

        // Act
        var result = new ReadMetaDataValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new ReadMetaData(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = new ReadMetaDataValidator().Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}