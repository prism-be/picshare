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
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Albums;
using Prism.Picshare.UnitTests;
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
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Albums, EntityReference.ComputeKey(request.OrganisationId, request.AlbumId), new Album());

        // Act
        var handler = new AddPictureToAlbumHandler(storeClient.Object);
        var album = await handler.Handle(request, CancellationToken.None);

        // Assert
        album.Pictures.Should().Contain(pictureId);
        storeClient.VerifySaveState<Album>(Stores.Albums);
    }
}