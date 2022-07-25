// -----------------------------------------------------------------------
//  <copyright file = "DaprBlobClientTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Services.Dapr;
using Xunit;

namespace Prism.Picshare.Tests.Dapr;

public class DaprBlobClientTests
{
    [Fact]
    public async Task List_Ok()
    {
        // Arrange
        var doc = JsonDocument.Parse("[\"somestring1\", \"somestring2\"]");
        var json = JsonSerializer.Serialize(doc);
        var jsonBytes = Encoding.Default.GetBytes(json);
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), CancellationToken.None))
            .ReturnsAsync(new BindingResponse(new BindingRequest(Stores.Data, "list"), jsonBytes, new Dictionary<string, string>()));
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprBlobClient(daprClient.Object, telemetryClient);
        var dataRead = await storeClient.ListAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        dataRead.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Read_Ok()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var blobName = Guid.NewGuid().ToString();
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), CancellationToken.None))
            .ReturnsAsync(new BindingResponse(new BindingRequest(Stores.Data, "get"), data, new Dictionary<string, string>()));
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprBlobClient(daprClient.Object, telemetryClient);
        var dataRead = await storeClient.ReadAsync(blobName, CancellationToken.None);

        // Assert
        dataRead.Should().BeEquivalentTo(data);
    }
    
    [Fact]
    public async Task Create_Ok()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var blobName = Guid.NewGuid().ToString();
        var daprClient = new Mock<DaprClient>();
        var telemetryClient = new TelemetryClient(new TelemetryConfiguration());

        // Act
        var storeClient = new DaprBlobClient(daprClient.Object, telemetryClient);
        await storeClient.CreateAsync(blobName, data, CancellationToken.None);

        // Assert
        daprClient.Verify(x => x.InvokeBindingAsync(Stores.Data, "create", Convert.ToBase64String(data), It.IsAny<Dictionary<string, string>>(), CancellationToken.None));
    }
}