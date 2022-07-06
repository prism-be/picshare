// -----------------------------------------------------------------------
//  <copyright file = "SaveCommandBehaviourTests.cs" company = "Prism">
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
using Prism.Picshare.Services.Pictures.Commands;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Configuration;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands;

public class SaveCommandBehaviourTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new InitializePicture(Guid.NewGuid(), Guid.NewGuid(), PictureSource.Upload);
        var daprClient = new Mock<DaprClient>();

        // Act
        var behavior = new SaveCommandBehaviour<InitializePicture, Picture>(daprClient.Object);
        var reponse = await behavior.Handle(request, CancellationToken.None, () => Task.FromResult(new Picture
        {
            Id = request.PictureId
        }));

        // Assert
        reponse.Id.Should().Be(request.PictureId);
        daprClient.Verify(x =>
            x.SaveStateAsync(Stores.Commands, It.IsAny<string>(), It.IsAny<CommandHistory<InitializePicture>>(), It.IsAny<StateOptions>(), It.IsAny<IReadOnlyDictionary<string, string>>(),
                CancellationToken.None));
    }
}