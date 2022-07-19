// -----------------------------------------------------------------------
//  <copyright file = "PublisherClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Text.Json;
using Dapr.Client;
using Microsoft.ApplicationInsights;

namespace Prism.Picshare.Dapr;

public interface IPublisherClient
{
    Task Publish<T>(string topic, T data, CancellationToken cancellationToken = default);
}

public class PublisherClient : IPublisherClient
{
    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public PublisherClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public async Task Publish<T>(string topic, T data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            await _daprClient.PublishEventAsync(Publishers.PubSub, topic, data, cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("PUBSUB", topic, JsonSerializer.Serialize(data), startTime, watch.Elapsed, success);
        }
    }
}