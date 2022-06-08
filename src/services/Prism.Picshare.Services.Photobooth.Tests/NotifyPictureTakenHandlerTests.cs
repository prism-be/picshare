// -----------------------------------------------------------------------
//  <copyright file="NotifyPictureTakenHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Photobooth.Commands;
using Prism.Picshare.Services.Photobooth.Hubs;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Tests;

public class NotifyPictureTakenHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var picture = new PhotoboothPicture
        {
            Id = Guid.NewGuid()
        };

        var all = new Mock<IClientProxy>();
        var clients = new Mock<IHubClients>();
        clients.Setup(x => x.All).Returns(all.Object);
        var hub = new Mock<IHubContext<PhotoboothHub>>();
        hub.Setup(x => x.Clients).Returns(clients.Object);

        var logger = new Mock<ILogger<NotifyPictureTakenHandler>>();

        var handler = new NotifyPictureTakenHandler(hub.Object, logger.Object);

        // Act
        var result = await handler.Handle(new NotifyPictureTaken(picture), CancellationToken.None);

        // Assert
        Assert.Equal(picture.Id, result.Id);
    }
}