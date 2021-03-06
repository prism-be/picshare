// -----------------------------------------------------------------------
//  <copyright file = "StoreClientTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Dapr;
using Xunit;

namespace Prism.Picshare.Tests.Dapr;

public class StoreClientTests
{

    [Fact]
    public async Task Get_NoStore()
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
        var act = async () => await storeClient.GetStateAsync<dynamic>(data.Id.ToString());

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
    }

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
        daprClient.Setup(x =>
                x.GetStateAsync<User>("state" + Stores.Users, data.OrganisationId + "+" + data.Id, null, It.IsAny<IReadOnlyDictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        var result = await storeClient.GetStateNullableAsync<User>(data.OrganisationId, data.Id);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Get_WithStore_Ok()
    {
        // Arrange
        var data = new Album
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x =>
                x.GetStateAsync<Album>("state" + Stores.Albums, data.OrganisationId + "+" + data.Id, null, It.IsAny<Dictionary<string, string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        var result = await storeClient.GetStateAsync<Album>(data.OrganisationId, data.Id);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Save_NoStore()
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
        var act = async () => await storeClient.SaveStateAsync(data.Id.ToString(), new DummyRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        await act.Should().ThrowAsync<NotImplementedException>();
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
        await storeClient.SaveStateAsync(data);

        // Assert
        daprClient.Verify(x => x.SaveStateAsync("state" + Stores.Users, data.OrganisationId + "+" + data.Id, data, null, It.IsAny<IReadOnlyDictionary<string, string>>(),
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Save_WithStore_Ok()
    {
        // Arrange
        var data = new Album
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };
        var daprClient = new Mock<DaprClient>();
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprStoreClient(daprClient.Object, telemetryClient);
        await storeClient.SaveStateAsync(data);

        // Assert
        daprClient.Verify(x => x.SaveStateAsync("state" + Stores.Albums, data.OrganisationId + "+" + data.Id, data, null, It.IsAny<IReadOnlyDictionary<string, string>>(),
            It.IsAny<CancellationToken>()));
    }
}