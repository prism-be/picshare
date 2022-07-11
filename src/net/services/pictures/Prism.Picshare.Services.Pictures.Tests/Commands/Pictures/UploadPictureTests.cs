// -----------------------------------------------------------------------
//  <copyright file = "UploadPictureTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class UploadPictureTests
{

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        var data = new byte[42];
        Random.Shared.NextBytes(data);

        // Act
        var handler = new UploadPictureHandler(Mock.Of<ILogger<UploadPictureHandler>>(), daprClient.Object);
        var result = await handler.Handle(new UploadPicture(Guid.NewGuid(), Guid.NewGuid(), data), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        daprClient.VerifyPublishEvent<EntityReference>(Publishers.PubSub, Topics.Pictures.Uploaded);
    }

    [Fact]
    public void Validate_Empty_Data()
    {
        // Arrange
        var data = new byte[42];
        var request = new UploadPicture(Guid.NewGuid(), Guid.NewGuid(), data);

        // Act
        var validator = new UploadPictureValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Empty_Organisation()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var request = new UploadPicture(Guid.Empty, Guid.NewGuid(), data);

        // Act
        var validator = new UploadPictureValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var request = new UploadPicture(Guid.NewGuid(), Guid.NewGuid(), data);

        // Act
        var validator = new UploadPictureValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}