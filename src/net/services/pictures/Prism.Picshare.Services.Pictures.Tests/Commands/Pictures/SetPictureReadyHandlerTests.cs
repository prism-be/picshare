// -----------------------------------------------------------------------
//  <copyright file = "SetPictureReadyHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture
        {
            Summary = new PictureSummary()
        });

        // Act
        var handler = new SetPictureReadyHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new SetPictureReady(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Ready.Should().BeTrue();
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }
}