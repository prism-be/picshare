// -----------------------------------------------------------------------
//  <copyright file = "SetPictureNameHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Xunit;
using Stores = Prism.Picshare.Services.Pictures.Configuration.Stores;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class SetPictureNameHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new SetPictureName(Guid.NewGuid(), Guid.NewGuid(), "Hellow World.png");
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new SetPictureNameHandler(daprClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.Id.Should().Be(request.PictureId);
        picture.Name.Should().Be("Hellow World.png");
        daprClient.VerifySaveState<Picture>(Stores.Pictures);
        daprClient.VerifyPublishEvent<EntityReference>(Publishers.PubSub, Topics.Pictures.Updated);
    }
}