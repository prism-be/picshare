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
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Configuration;
using Xunit;

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
        daprClient.Verify(x =>
            x.SaveStateAsync(Stores.Pictures, It.IsAny<string>(), It.IsAny<Picture>(), It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(), CancellationToken.None));
    }
}