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
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class IncreaseViewCountHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new IncreaseViewCount(Guid.NewGuid(), Guid.NewGuid());
        var storeClient = new Mock<StoreClient>();

        // Act
        var handler = new IncreaseViewCountHandler(storeClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.Views.Should().Be(1);
        storeClient.VerifySaveState<Picture>(Stores.Pictures);
    }
}