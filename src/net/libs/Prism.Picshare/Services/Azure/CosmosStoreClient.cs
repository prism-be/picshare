// -----------------------------------------------------------------------
//  <copyright file = "CosmosStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Text;
using System.Text.Json;
using Azure;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Azure;

public class CosmosStoreClient : StoreClient
{
    private readonly Database _database;
    private readonly ILogger<CosmosStoreClient> _logger;

    public CosmosStoreClient(Database database, ILogger<CosmosStoreClient> logger)
    {
        _database = database;
        _logger = logger;
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
        await MutateStateAsync(0, store, organisationId, id, mutation, cancellationToken);
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

    private async Task MutateStateAsync<T>(int retries, string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default) where T : EntityId
    {
        while (true)
        {
            if (retries > 30)
            {
                throw new StoreAccessException("Cannot get lock on ressource", $"{store}|{organisationId}|{id}");
            }

            var lockedItem = await TryGetLock<T>(store, organisationId, id);

            if (lockedItem == null)
            {
                Thread.Sleep(1000);
                retries++;
                continue;
            }

            try
            {
                mutation(lockedItem);
                await SaveStateAsync(store, organisationId, id, lockedItem, cancellationToken);
            }
            finally
            {
                ReleaseLock(store, organisationId, id);
            }

            break;
        }
    }

    private static void ReleaseLock(string store, string organisationId, string id)
    {
        var key = $"{store}-{organisationId}-{id}";

        var container = new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_BLOB_CONNECTION_STRING"), "picshare");
        var blob = container.GetBlobClient(key);
        blob.Delete();
    }

    private async Task<T?> TryGetLock<T>(string store, string organisationId, string id)
        where T : EntityId
    {
        var key = $"{store}-{organisationId}-{id}";

        var container = new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_BLOB_CONNECTION_STRING"), "picshare");
        var blob = container.GetBlobClient(key);

        if (await blob.ExistsAsync())
        {
            _logger.LogWarning("Ressource is locked : {key}", key);
            return null;
        }

        try
        {
            await blob.UploadAsync(new MemoryStream(Encoding.Default.GetBytes(key)));
        }
        catch (RequestFailedException e)
        {
            _logger.LogWarning(e, "Lock was already took in the meantime : {key}", key);
            return null;
        }

        var item = await GetStateNullableAsync<T>(store, organisationId, id);

        if (item == null)
        {
            return null;
        }

        return item;
    }
}