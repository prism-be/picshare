// -----------------------------------------------------------------------
//  <copyright file = "StoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Dapr.Client;
using Microsoft.ApplicationInsights;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Dapr;

public abstract class StoreClient
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
            typeof(MailAction<>), Stores.MailActions
        },
        {
            typeof(Organisation), Stores.Organisations
        },
        {
            typeof(Picture), Stores.Pictures
        },
        {
            typeof(User), Stores.Users
        }
    };

    public async Task<T> GetStateAsync<T>(string key, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(key, cancellationToken);
        return result ?? new T();
    }

    public async Task<T> GetStateAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(store, key, cancellationToken);
        return result ?? new T();
    }

    public abstract Task<T?> GetStateNullableAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : class;

    public async Task<T?> GetStateNullableAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateNullableAsync<T>(store, key, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public abstract Task SaveStateAsync<T>(string store, string key, T data, CancellationToken cancellationToken = default);

    public async Task SaveStateAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            await SaveStateAsync(store, key, data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }
}

public sealed class DaprStoreClient : StoreClient
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

            var result = await _daprClient.GetStateAsync<T>(store, key, cancellationToken: cancellationToken);
            success = true;
            return result;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE-GET", store, key, startTime, watch.Elapsed, success);
        }
    }

    public override async Task SaveStateAsync<T>(string store, string key, T data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            await _daprClient.SaveStateAsync(store, key, data, cancellationToken: cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE-SET", store, key, startTime, watch.Elapsed, success);
        }
    }
}