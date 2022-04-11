// -----------------------------------------------------------------------
//  <copyright file="PictureTakenHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Data;
using Prism.Picshare.Events;
using Prism.Picshare.Photobooth.Commands;
using Prism.Picshare.Photobooth.Handlers;
using Prism.Picshare.Photobooth.Model;
using Xunit;

namespace Prism.Picshare.Photobooth.Tests.Commands;

public class PictureTakenHandlerTests
{
    [Fact]
    public void Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid().ToString();
        var sessionId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();

        var db = new Mock<IDatabase>();

        var databaseResolver = new Mock<IDatabaseResolver>();
        databaseResolver.Setup(x => x.GetDatabase(organisationId, "photobooth")).Returns(db.Object);

        var eventPublisher = new Mock<IEventPublisher>();

        var handler = new PictureTakenHandler(databaseResolver.Object, eventPublisher.Object, Mock.Of<ILogger<PictureTakenHandler>>());

        // Act
        var request = new PictureTaken(organisationId, sessionId, pictureId);
        handler.Handle(request, default);

        // Assert
        db.Verify(x => x.Insert(It.IsAny<Pictures>()), Times.Once);
        eventPublisher.Verify(x => x.Publish(Topics.Photobooth.PictureTaken, It.IsAny<PictureTaken>()), Times.Once);
    }
}