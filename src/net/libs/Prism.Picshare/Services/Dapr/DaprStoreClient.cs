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

    public override async Task<T?> GetStateNullableAsync<T>(string store, string organisation, string id, CancellationToken cancellationToken = default) where T : class
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        var key = id;

        if (!string.IsNullOrWhiteSpace(organisation))
        {
            key = $"{organisation}+{id}";
        }

        try
        {
            var metaData = new Dictionary<string, string>();

            var result = await _daprClient.GetStateAsync<T>("state" + store, key, metadata: metaData, cancellationToken: cancellationToken);
            success = true;
            return result;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE", store, "GET" + key, startTime, watch.Elapsed, success);
        }
    }

    public override Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override async Task SaveStateAsync<T>(string store, string organisation, string id, T data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        var key = id;

        if (!string.IsNullOrWhiteSpace(organisation))
        {
            key = $"{organisation}+{id}";
        }

        try
        {
            var metaData = new Dictionary<string, string>();

            if (data is EntityReference entityReference)
            {
                metaData.Add("partitionKey", entityReference.OrganisationId.ToString());
            }

            await _daprClient.SaveStateAsync("state" + store, key, data, metadata: metaData, cancellationToken: cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE", store, "SET" + key, startTime, watch.Elapsed, success);
        }
    }
}