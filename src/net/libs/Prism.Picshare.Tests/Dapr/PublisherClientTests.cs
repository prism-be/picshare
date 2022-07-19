﻿// -----------------------------------------------------------------------
//  <copyright file = "PublisherClientTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Xunit;

namespace Prism.Picshare.Tests.Dapr;

public class PublisherClientTests
{
    [Fact]
    public async Task Publish_Ok()
    {
        // Arrange
        var topic = Guid.NewGuid().ToString();
        var data = new EntityReference
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var publisherClient = new PublisherClient(daprClient.Object, telemetryClient);
        await publisherClient.PublishEventAsync(topic, data);

        // Assert
        daprClient.Verify(x => x.PublishEventAsync(Publishers.PubSub, topic, data, It.IsAny<CancellationToken>()));
    }
}