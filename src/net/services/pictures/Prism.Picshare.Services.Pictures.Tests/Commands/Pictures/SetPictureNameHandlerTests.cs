// -----------------------------------------------------------------------
//  <copyright file = "SetPictureNameHandlerTests.cs" company = "Prism">
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

public class SetPictureNameHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new SetPictureName(Guid.NewGuid(), Guid.NewGuid(), "Hellow World.png");
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();

        // Act
        var handler = new SetPictureNameHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.Id.Should().Be(request.PictureId);
        picture.Name.Should().Be("Hellow World.png");
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
        publisherClient.VerifyPublishEvent<EntityReference>(Topics.Pictures.Updated);
    }
}