// -----------------------------------------------------------------------
//  <copyright file = "AddPictureToAlbumHandlerTests.cs" company = "Prism">
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
using Prism.Picshare.Services.Pictures.Commands.Albums;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Albums;

public class AddPictureToAlbumHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var pictureId = Guid.NewGuid();
        var request = new AddPictureToAlbum(Guid.NewGuid(), Guid.NewGuid(), pictureId);
        var storeClient = new Mock<IStoreClient>();

        // Act
        var handler = new AddPictureToAlbumHandler(storeClient.Object);
        var album = await handler.Handle(request, CancellationToken.None);

        // Assert
        album.Pictures.Should().Contain(pictureId);
        daprClient.Verify(x =>
            x.SaveStateAsync(Stores.Albums, It.IsAny<string>(), It.IsAny<Album>(), It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(), CancellationToken.None));
    }
}