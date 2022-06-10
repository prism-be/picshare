// -----------------------------------------------------------------------
//  <copyright file="PictureWatcherTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Exceptions;
using Prism.Picshare.Services.Photobooth.Services;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Tests.Services;

public class PictureWatcherTests
{

    [Fact]
    public async Task Execute_Missing_PHOTOBOOTH_ORGANISATION()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {
                "PHOTOBOOTH_SESSION", Guid.NewGuid().ToString()
            }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = new Mock<ILogger<PictureWatcher>>();
        var env = new HostingEnvironment
        {
            ContentRootPath = Path.GetTempPath()
        };

        var daprClient = new Mock<DaprClient>();

        var watcher = new PictureWatcher(logger.Object, env, config, daprClient.Object);

        // Act
        var ex = await Assert.ThrowsAsync<MissingConfigurationException>(async () => await watcher.StartAsync(CancellationToken.None));

        // Assert
        Assert.Equal("Environment variable PHOTOBOOTH_ORGANISATION not found, cannot continue.", ex.Message);
    }

    [Fact]
    public async Task Execute_Missing_PHOTOBOOTH_SESSION()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {
                "PHOTOBOOTH_ORGANISATION", Guid.NewGuid().ToString()
            }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = new Mock<ILogger<PictureWatcher>>();
        var env = new HostingEnvironment
        {
            ContentRootPath = Path.GetTempPath()
        };

        var daprClient = new Mock<DaprClient>();

        var watcher = new PictureWatcher(logger.Object, env, config, daprClient.Object);

        // Act
        var ex = await Assert.ThrowsAsync<MissingConfigurationException>(async () => await watcher.StartAsync(CancellationToken.None));

        // Assert
        Assert.Equal("Environment variable PHOTOBOOTH_SESSION not found, cannot continue.", ex.Message);
    }

    [Fact]
    public async Task Execute_Ok()
    {
        var organisationId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {
                "PHOTOBOOTH_ORGANISATION", organisationId.ToString()
            },
            {
                "PHOTOBOOTH_SESSION", sessionId.ToString()
            }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = new Mock<ILogger<PictureWatcher>>();
        var env = new HostingEnvironment
        {
            ContentRootPath = Path.GetTempPath()
        };

        var daprClient = new Mock<DaprClient>();

        var watcher = new PictureWatcher(logger.Object, env, config, daprClient.Object);

        // Act
        await watcher.StartAsync(CancellationToken.None);

        // Assert
        Assert.Equal(organisationId, watcher.OrganisationId);
        Assert.Equal(sessionId, watcher.SessionId);
    }

    [Fact]
    public async Task ProcessPictureAsync_Ok()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            {
                "PHOTOBOOTH_ORGANISATION", Guid.NewGuid().ToString()
            },
            {
                "PHOTOBOOTH_SESSION", Guid.NewGuid().ToString()
            }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var logger = new Mock<ILogger<PictureWatcher>>();
        var env = new HostingEnvironment
        {
            ContentRootPath = Path.GetTempPath()
        };

        var daprClient = new Mock<DaprClient>();

        var watcher = new PictureWatcher(logger.Object, env, config, daprClient.Object);
        var file = Path.GetTempFileName();

        // Act
        await watcher.ProcessPictureAsync(file);

        // Assert
        daprClient.Verify(x => x.PublishEventAsync(DaprConfiguration.Photobooth.PubSub, Topics.Photobooth.PictureTaken, It.IsAny<PhotoboothPicture>(), It.IsAny<CancellationToken>()), Times.Once);
        daprClient.Verify(x => x.InvokeBindingAsync("datastore", "create", It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
        daprClient.Verify(x => x.PublishEventAsync(DaprConfiguration.PubSub, Topics.Photobooth.PictureUploaded, It.IsAny<PhotoboothPicture>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}