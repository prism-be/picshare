﻿// -----------------------------------------------------------------------
//  <copyright file = "ReadMetaDataTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Processor;

public class ReadMetaDataTests
{

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var publisherClient = new Mock<PublisherClient>();
        var blobClient = new Mock<BlobClient>();
        blobClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImage);

        // Act
        var handler = new ReadMetaDataHandler(blobClient.Object, publisherClient.Object);
        var result = await handler.Handle(new ReadMetaData(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().Be(ResultCodes.Ok);
        publisherClient.VerifyPublishEvent<Picture>(Topics.Pictures.ExifRead);
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