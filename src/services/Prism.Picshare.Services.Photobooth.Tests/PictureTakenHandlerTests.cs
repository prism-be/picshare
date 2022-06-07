// -----------------------------------------------------------------------
//  <copyright file="PictureTakenHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Logging;
using Moq;
using Pose;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Photobooth.Commands;
using Shimmy;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Tests;

public class PictureTakenHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        var logger = new Mock<ILogger<PictureTakenHandler>>();

        var handler = new PictureTakenHandler(logger.Object, daprClient.Object);

        var organisationId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        // Act

        var result = await handler.Handle(new PictureTaken(organisationId, sessionId), CancellationToken.None);

        // Assert
        daprClient.Verify(x => x.PublishEventAsync(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken, It.IsAny<PhotoboothPicture>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(organisationId, result.OrganisationId);
        Assert.Equal(sessionId, result.SessionId);
    }
}