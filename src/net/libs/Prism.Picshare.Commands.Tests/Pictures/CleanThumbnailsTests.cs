// -----------------------------------------------------------------------
//  <copyright file = "CleanThumbnailsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Services;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class CleanThumbnailsTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var blobClient = new Mock<BlobClient>();
        blobClient.Setup(x => x.ListAsync(organisationId, pictureId, CancellationToken.None))
            .ReturnsAsync(new List<string>
            {
                Guid.NewGuid() + "/source.jpg",
                Guid.NewGuid() + "/thumb1.webp",
                Guid.NewGuid() + "/thumb2.jpg",
                Guid.NewGuid() + "/thumb3.plip"
            });

        // Act
        var handler = new CleanThumbnailsHandler(Mock.Of<ILogger<CleanThumbnailsHandler>>(), blobClient.Object);
        await handler.Handle(new CleanThumbnails(organisationId, pictureId), CancellationToken.None);

        // Assert
        blobClient.Verify(x => x.DeleteAsync(It.IsAny<string>(), CancellationToken.None), Times.Exactly(3));
    }
}