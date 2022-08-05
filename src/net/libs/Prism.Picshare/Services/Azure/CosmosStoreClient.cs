// -----------------------------------------------------------------------
//  <copyright file = "CosmosStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Azure;

public class CosmosStoreClient : StoreClient
{
    private readonly Database _database;
    private readonly RedisLocker _locker;

    private readonly ILogger<CosmosStoreClient> _logger;

    public CosmosStoreClient(Database database, ILogger<CosmosStoreClient> logger, RedisLocker locker)
    {
        _database = database;
        _logger = logger;
        _locker = locker;
    }

    public override async Task<T?> GetStateNullableAsync<T>(string store, string organisation, string id, CancellationToken cancellationToken = default) where T : class
    {
        var partitionKey = id;

        if (!string.IsNullOrWhiteSpace(organisation))
        {
            partitionKey = organisation;
        }

        var container = _database.GetContainer(store);

        try
        {
            var partitionKeyObject = new PartitionKey(partitionKey);
            var item = await container.ReadItemAsync<T>(id, partitionKeyObject, cancellationToken: cancellationToken);

            if (item.StatusCode == HttpStatusCode.OK)
            {
                return item.Resource;
            }
        }
        catch (CosmosException e)
        {
            _logger.LogError(e, "Cannot read itemin cosmos DB");

            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            throw;
        }

        return null;
    }

    public override async Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
    {
        var key = GetKey(store, organisationId, id);

        using var locker = _locker.GetLock(key);

        var item = await GetStateNullableAsync<T>(store, organisationId, id, cancellationToken);

        if (item == null)
        {
            throw new StoreAccessException("Cannot mutate inexisting item", $"{store}-{organisationId}-{id}");
        }

        mutation(item);
        await SaveStateAsync(store, organisationId, id, item, cancellationToken);

        locker.Release();
    }

    public override async Task SaveStateAsync<T>(string store, string organisation, string id, T data, CancellationToken cancellationToken = default)
    {
        var partitionKey = id;

        if (!string.IsNullOrWhiteSpace(organisation))
        {
            partitionKey = organisation;
        }

        var container = _database.GetContainer(store);

        var partitionKeyObject = new PartitionKey(partitionKey);
        using var memory = new MemoryStream();
        await JsonSerializer.SerializeAsync(memory, data, cancellationToken: cancellationToken);
        var reponse = await container.UpsertItemStreamAsync(memory, partitionKeyObject, cancellationToken: cancellationToken);

        if (!reponse.IsSuccessStatusCode)
        {
            throw new StoreAccessException($"Cannot save item into CosmosDB ({reponse.StatusCode} - {reponse.ErrorMessage}", id);
        }
    }

    private static string GetKey(string store, string organisationId, string id)
    {
        return $"locks/{store}/{organisationId}/{id}";
    }
}