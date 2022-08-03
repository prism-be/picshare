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
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class SetPictureReadyHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture
        {
            Summary = new PictureSummary()
        });

        // Act
        var handler = new SetPictureReadyHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new SetPictureReady(organisationId, pictureId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Ready.Should().BeTrue();
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }
}