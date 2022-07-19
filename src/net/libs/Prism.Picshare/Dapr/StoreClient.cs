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

public interface IStoreClient
{
    Task<T> GetStateAsync<T>(string key, CancellationToken cancellationToken = default) where T : new();
    Task<T> GetStateAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : new();

    Task<Flow> GetStateFlowAsync(Guid organisationId, CancellationToken cancellationToken = default);
    Task<Picture> GetStatePictureAsync(Guid organisationId, Guid pictureId, CancellationToken cancellationToken = default);

    Task SaveStateAsync<T>(string key, T data, CancellationToken cancellationToken = default);
    Task SaveStateAsync<T>(string store, string key, T data, CancellationToken cancellationToken = default);

    Task SaveStateAsync(Album data, CancellationToken cancellationToken = default);
    Task SaveStateAsync(Credentials data, CancellationToken cancellationToken = default);
    Task SaveStateAsync(Flow data, CancellationToken cancellationToken = default);
    Task SaveStateAsync<T>(MailAction<T> data, CancellationToken cancellationToken = default);
    Task SaveStateAsync(Organisation data, CancellationToken cancellationToken = default);
    Task SaveStateAsync(Picture data, CancellationToken cancellationToken = default);
    Task SaveStateAsync(User data, CancellationToken cancellationToken = default);
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

    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public StoreClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public async Task<T> GetStateAsync<T>(string key, CancellationToken cancellationToken = default) where T : new()
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateAsync<T>(store, key, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task<T> GetStateAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : new()
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            success = true;
            var result = await _daprClient.GetStateAsync<T>(store, key, cancellationToken: cancellationToken);

            return result ?? new T();
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE-GET", store, key, startTime, watch.Elapsed, success);
        }
    }

    public async Task<Flow> GetStateFlowAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        return await GetStateAsync<Flow>(organisationId.ToString(), cancellationToken);
    }

    public async Task<Picture> GetStatePictureAsync(Guid organisationId, Guid pictureId, CancellationToken cancellationToken = default)
    {
        return await GetStateAsync<Picture>(EntityReference.ComputeKey(organisationId, pictureId), cancellationToken);
    }

    public async Task SaveStateAsync<T>(string key, T data, CancellationToken cancellationToken = default)
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            await SaveStateAsync(store, key, data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task SaveStateAsync<T>(string store, string key, T data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            success = true;
            await _daprClient.SaveStateAsync(store, key, data, cancellationToken: cancellationToken);
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("STORE-SET", store, key, startTime, watch.Elapsed, success);
        }
    }

    public async Task SaveStateAsync(Album data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Key, data, cancellationToken);
    }

    public async Task SaveStateAsync(Credentials data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Login, data, cancellationToken);
    }

    public async Task SaveStateAsync(Flow data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.OrganisationId.ToString(), data, cancellationToken);
    }

    public async Task SaveStateAsync<T>(MailAction<T> data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Key, data, cancellationToken);
    }

    public async Task SaveStateAsync(Organisation data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Id.ToString(), data, cancellationToken);
    }

    public async Task SaveStateAsync(Picture data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Key, data, cancellationToken);
    }

    public async Task SaveStateAsync(User data, CancellationToken cancellationToken = default)
    {
        await SaveStateAsync(data.Key, data, cancellationToken);
    }
}