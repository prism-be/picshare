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
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class SetPictureNameHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var request = new SetPictureName(organisationId, pictureId, "Hellow World.png");
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture()
        {
            Id = pictureId,
            OrganisationId = organisationId
        });

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