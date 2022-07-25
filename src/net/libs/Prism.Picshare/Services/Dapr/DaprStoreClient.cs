// -----------------------------------------------------------------------
//  <copyright file = "DaprStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Dapr.Client;
using Microsoft.ApplicationInsights;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Dapr;

public class DaprStoreClient : StoreClient
{
    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public DaprStoreClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public override async Task<T?> GetStateNullableAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : class
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            var metaData = new Dictionary<string, string>();

            if (OrganisationScopedTypes.Contains(typeof(T)))
            {
                var organisationId = key.Split('+')[0];
                metaData.Add("partitionKey", organisationId);
            }

            var result = await _daprClient.GetStateAsync<T>(store, key, metadata: metaData, cancellationToken: cancellationToken);
            success = true;
            return result;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE", store, "GET" + key, startTime, watch.Elapsed, success);
        }
    }

    public override async Task SaveStateAsync<T>(string store, string key, T data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            var metaData = new Dictionary<string, string>();

            if (data is EntityReference entityReference)
            {
                metaData.Add("partitionKey", entityReference.OrganisationId.ToString());
            }

            await _daprClient.SaveStateAsync(store, key, data, metadata: metaData, cancellationToken: cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE", store, "SET" + key, startTime, watch.Elapsed, success);
        }
    }
}