// -----------------------------------------------------------------------
//  <copyright file = "RelaunchUploadHandlerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Services.Pictures.Commands.Admin;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Admin;

public class RelaunchUploadHandlerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var publisherClient = new Mock<IPublisherClient>();
        var blobClient = new Mock<IBlobClient>();
        blobClient.Setup(x => x.ListAsync(organisationId, default))
            .ReturnsAsync(new List<string>());

        // Act
        var handler = new RelaunchUploadHandler(blobClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RelaunchUpload(organisationId), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
    }
}