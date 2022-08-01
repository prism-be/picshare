// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCountHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class IncreaseViewCountHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var request = new IncreaseViewCount(organisationId, pictureId);
        var storeClient = new Mock<StoreClient>();
        var picture = new Picture
        {
            Views = 42
        };
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, picture);

        // Act
        var handler = new IncreaseViewCountHandler(storeClient.Object);
        picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.Views.Should().Be(43);
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
    }
}