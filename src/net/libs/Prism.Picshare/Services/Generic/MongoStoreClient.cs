﻿// -----------------------------------------------------------------------
//  <copyright file = "MongoStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Generic;

public class MongoStoreClient : StoreClient
{
    private readonly RedisLocker _locker;
    private readonly IMongoClient _mongoClient;

    public MongoStoreClient(IMongoClient mongoClient, RedisLocker locker)
    {
        _mongoClient = mongoClient;
        _locker = locker;
    }

    public override async Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class
    {
        var db = _mongoClient.GetDatabase("picshare");
        var collection = db.GetCollection<T>(store);
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await result.SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }

    public override async Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
    {
        using var locked = _locker.GetLock(id);

        var item = await GetStateNullableAsync<T>(store, organisationId, id, cancellationToken);

        if (item == null)
        {
            throw new StoreAccessException("Cannot mutate inexisting item", $"{store}-{organisationId}-{id}");
        }

        mutation(item);
        await SaveStateAsync(store, organisationId, id, item, cancellationToken);

        locked.Release();
    }

    public override async Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default)
    {
        var db = _mongoClient.GetDatabase("picshare");
        var collection = db.GetCollection<T>(store);

        var existing = await collection.FindAsync(Builders<T>.Filter.Eq("Id", id), cancellationToken: cancellationToken);

        if (await existing.AnyAsync(cancellationToken: cancellationToken))
        {
            await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), data, cancellationToken: cancellationToken);
            return;
        }
        
        await collection.InsertOneAsync(data, cancellationToken: cancellationToken);
    }
}