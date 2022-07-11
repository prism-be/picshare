// -----------------------------------------------------------------------
//  <copyright file = "IncreaseViewCountHandlerTests.cs" company = "Prism">
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
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class IncreaseViewCountHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new IncreaseViewCount(Guid.NewGuid(), Guid.NewGuid());
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new IncreaseViewCountHandler(daprClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.Views.Should().Be(1);
        daprClient.Verify(x =>
            x.SaveStateAsync(Stores.Pictures, It.IsAny<string>(), It.IsAny<Picture>(), It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(), CancellationToken.None));
    }
}