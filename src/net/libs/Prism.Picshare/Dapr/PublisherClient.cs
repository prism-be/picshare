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

public abstract class PublisherClient
{
    public abstract Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default);
}

public class DaprPublisherClient : PublisherClient
{
    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public DaprPublisherClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public override async Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default)
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

            _telemetryClient.TrackDependency("PUBSUB", Publishers.PubSub, topic , startTime, watch.Elapsed, success);
        }
    }
}