// -----------------------------------------------------------------------
//  <copyright file = "StoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Dapr.Client;
using Microsoft.ApplicationInsights;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Dapr;

public interface IStoreClient
{
    Task<T> Get<T>(string key, CancellationToken cancellationToken = default);
    Task<T> Get<T>(string store, string key, CancellationToken cancellationToken = default);
}

public class StoreClient : IStoreClient
{
    private static readonly Dictionary<Type, string> StoresMatching = new()
    {
        {
            typeof(Album), Stores.Albums
        },
        {
            typeof(Credentials), Stores.Credentials
        },
        {
            typeof(Flow), Stores.Flow
        },
        {
            typeof(Organisation), Stores.Organisations
        },
        {
            typeof(Topics.Pictures), Stores.Pictures
        },
        {
            typeof(User), Stores.Users
        }
    };

    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public StoreClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public async Task<T> Get<T>(string key, CancellationToken cancellationToken = default)
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await Get<T>(store, key, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task<T> Get<T>(string store, string key, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            success = true;
            return await _daprClient.GetStateAsync<T>(store, key, cancellationToken: cancellationToken);
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE-GET", store, key, startTime, watch.Elapsed, success);
        }
    }
}