// -----------------------------------------------------------------------
//  <copyright file="PictureRepositoryTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Moq;
using Prism.Picshare.Data.CosmosDB;
using Prism.Picshare.Data.Tests.Fakes;
using Xunit;

namespace Prism.Picshare.Data.Tests.CosmosDB;

public class PictureRepositoryTests
{
    [Fact]
    public async Task Upsert_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var picture = new Picture
        {
            Id = Guid.NewGuid()
        };

        var containerMock = new Mock<Container>();
        var organisationRepository = new PictureRepository(new FakeCosmosClient(containerMock.Object));

        // Act
        var result = await organisationRepository.Upsert(organisationId, picture);

        // Assert
        Assert.Equal(HttpStatusCode.Created, result);
        containerMock.Verify(x => x.UpsertItemAsync(picture, new PartitionKey(organisationId.ToString()), null, default), Times.Once);
    }

    [Fact]
    public async Task Get_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var picture = new Picture
        {
            Id = Guid.NewGuid()
        };

        var containerMock = new Mock<Container>();
        containerMock.Setup(x => x.ReadItemAsync<Picture>(picture.Id.ToString(), new PartitionKey(organisationId.ToString()), null, default)).ReturnsAsync(new FakeItemResponse<Picture>(HttpStatusCode.OK, picture));
        var organisationRepository = new PictureRepository(new FakeCosmosClient(containerMock.Object));

        // Act
        var result = await organisationRepository.Get(organisationId, picture.Id);

        // Assert
        Assert.Equal(picture, result);
    }

    [Fact]
    public async Task Get_NoFound()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var picture = new Picture
        {
            Id = Guid.NewGuid()
        };

        var containerMock = new Mock<Container>();
        containerMock.Setup(x => x.ReadItemAsync<Picture>(It.IsAny<string>(), It.IsAny<PartitionKey>(), null, default)).ReturnsAsync(new FakeItemResponse<Picture>(HttpStatusCode.NotFound));
        var organisationRepository = new PictureRepository(new FakeCosmosClient(containerMock.Object));

        // Act
        var result = await organisationRepository.Get(organisationId, picture.Id);

        // Assert
        Assert.Null(result);
    }
}