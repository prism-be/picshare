// -----------------------------------------------------------------------
//  <copyright file = "StoreClientTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Xunit;

namespace Prism.Picshare.Tests.Dapr;

public class StoreClientTests
{

    [Fact]
    public async Task Get_Ok()
    {
        // Arrange
        var data = new User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.GetStateAsync<User>(Stores.Users, data.Key, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        var result = await storeClient.GetStateAsync<User>(data.Key);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Get_WithStore_Ok()
    {
        // Arrange
        var data = new EntityReference
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.GetStateAsync<EntityReference>(Stores.Albums, data.Key, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        var result = await storeClient.GetStateAsync<EntityReference>(Stores.Albums, data.Key);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Save_Ok()
    {
        // Arrange
        var data = new User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        await storeClient.SaveStateAsync(data.Key, data);

        // Assert
        daprClient.Verify(x => x.SaveStateAsync(Stores.Users, data.Key, data, null, null, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Save_WithStore_Ok()
    {
        // Arrange
        var data = new User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        await storeClient.SaveStateAsync(Stores.Albums, data.Key, data);

        // Assert
        daprClient.Verify(x => x.SaveStateAsync(Stores.Albums, data.Key, data, null, null, It.IsAny<CancellationToken>()));
    }
}