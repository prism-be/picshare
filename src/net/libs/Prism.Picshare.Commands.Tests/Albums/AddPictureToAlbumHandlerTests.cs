// -----------------------------------------------------------------------
//  <copyright file = "AddPictureToAlbumHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.Commands.Albums;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Albums;

public class AddPictureToAlbumHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var pictureId = Guid.NewGuid();
        var request = new AddPictureToAlbum(Guid.NewGuid(), Guid.NewGuid(), pictureId);
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Albums, request.OrganisationId, request.AlbumId, new Album());

        // Act
        var handler = new AddPictureToAlbumHandler(storeClient.Object);
        var album = await handler.Handle(request, CancellationToken.None);

        // Assert
        album.Pictures.Should().Contain(pictureId);
        storeClient.VerifySaveState<Album>(Stores.Albums);
    }
}