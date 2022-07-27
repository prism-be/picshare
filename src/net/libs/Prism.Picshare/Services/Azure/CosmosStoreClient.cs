// -----------------------------------------------------------------------
//  <copyright file = "CosmosStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Azure;

public class CosmosStoreClient : StoreClient
{
    private readonly Database _database;

    public CosmosStoreClient(Database database)
    {
        _database = database;
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
}