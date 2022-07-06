// -----------------------------------------------------------------------
//  <copyright file = "InitializePictureTests.cs" company = "Prism">
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

public class InitializePictureTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var pictureId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var request = new InitializePicture(organisationId, pictureId, PictureSource.Upload);
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new InitializePictureHandler(daprClient.Object);
        var picture = await handler.Handle(request, CancellationToken.None);

        // Assert
        picture.OrganisationId.Should().Be(organisationId);
        picture.Id.Should().Be(pictureId);
        daprClient.Verify(x =>
            x.SaveStateAsync(Stores.Pictures, It.IsAny<string>(), It.IsAny<Picture>(), It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(), CancellationToken.None));
    }
}