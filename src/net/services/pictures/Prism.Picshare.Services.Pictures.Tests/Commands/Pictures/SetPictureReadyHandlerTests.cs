// -----------------------------------------------------------------------
//  <copyright file = "SetPictureReadyHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class SetPictureReadyHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture
        {
            Summary = new PictureSummary()
        });

        // Act
        var handler = new SetPictureReadyHandler(daprClient.Object);
        var result = await handler.Handle(new SetPictureReady(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Ready.Should().BeTrue();
        daprClient.VerifySaveState<Picture>(Stores.Pictures);
        daprClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }
}