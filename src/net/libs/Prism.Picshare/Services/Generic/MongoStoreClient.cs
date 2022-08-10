// -----------------------------------------------------------------------
//  <copyright file = "MongoStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MongoDB.Driver;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Generic;

public class MongoStoreClient : BaseMutableStoreClient
{
    private readonly IMongoClient _mongoClient;

    public MongoStoreClient(IMongoClient mongoClient, RedisLocker locker) : base(locker)
    {
        _mongoClient = mongoClient;
    }

    public override async Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class
    {
        var db = _mongoClient.GetDatabase("picshare");
        var collection = db.GetCollection<T>(store);
        var filter = GetFilter<T>(id);
        var result = await collection.FindAsync(filter, cancellationToken: cancellationToken);

        return await result.SingleOrDefaultAsync(cancellationToken: cancellationToken);
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

    private static FilterDefinition<T> GetFilter<T>(string id) where T : class
    {
        var idProperty = typeof(T).GetProperty("Id");

        if (idProperty == null)
        {
            throw new StoreAccessException("Cannot find an item without Id property on type {type}", typeof(T).FullName!);
        }

        if (idProperty.PropertyType == typeof(Guid))
        {
            if (!Guid.TryParse(id, out var idObject))
            {
                throw new StoreAccessException("Cannot parse id for an item with Guid as Id, value : {value}", id);
            }

            return Builders<T>.Filter.Eq("Id", idObject);
        }

        if (idProperty.PropertyType == typeof(string))
        {
            return Builders<T>.Filter.Eq("Id", id);
        }

        throw new StoreAccessException("Cannot create a filter for property type {type}", idProperty.PropertyType.FullName!);
    }
}