// -----------------------------------------------------------------------
//  <copyright file = "InitializePictureTests.cs" company = "Prism">
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
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class InitializePictureTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var pictureId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var owner = Guid.NewGuid();
        var request = new InitializePicture(organisationId, owner, pictureId, PictureSource.Upload);
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();

        // Act
        var handler = new InitializePictureHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.OrganisationId.Should().Be(organisationId);
        picture.Id.Should().Be(pictureId);
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
        publisherClient.VerifyPublishEvent<EntityReference>(Topics.Pictures.Created);
    }
}