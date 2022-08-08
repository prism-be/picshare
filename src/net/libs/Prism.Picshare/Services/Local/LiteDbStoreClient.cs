// -----------------------------------------------------------------------
//  <copyright file = "LiteDbStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using LiteDB;
using Prism.Picshare.Exceptions;
using Prism.Picshare.Services.Generic;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Prism.Picshare.Services.Local;

public class LiteDbStoreClient : StoreClient
{
    private readonly RedisLocker _locker;

    public LiteDbStoreClient(RedisLocker locker)
    {
        _locker = locker;
    }

    public override Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class
    {
        using var db = new LiteDatabase(GetDatabaseConenctionString(organisationId));
        var collection = db.GetCollection<DataStorage<T>>(store);
        var data = collection.FindById(id)?.Data;

        if (data == null)
        {
            return Task.FromResult((T?)null);
        }
        
        return Task.FromResult(JsonSerializer.Deserialize<T>(data));
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

    public override Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default)
    {
        using var db = new LiteDatabase(GetDatabaseConenctionString(organisationId));
        var collection = db.GetCollection<DataStorage<T>>(store);
        collection.Upsert(new DataStorage<T>
        {
            Id = id,
            Data = JsonSerializer.Serialize(data)
        });
        collection.EnsureIndex(x => x.Id);
        return Task.CompletedTask;
    }

    private string GetDatabaseConenctionString(string organisationId)
    {
        var baseConnectionString = "Connection=shared;Filename=";

        if (string.IsNullOrWhiteSpace(organisationId))
        {
            return baseConnectionString + Path.Combine(EnvironmentConfiguration.GetMandatoryConfiguration("LITE_DB_DIRECTORY"), $"{Guid.Empty}.db");
        }

        return baseConnectionString + Path.Combine(EnvironmentConfiguration.GetMandatoryConfiguration("LITE_DB_DIRECTORY"), $"{organisationId}.db");
    }

    private class DataStorage<T>
    {
        public DataStorage()
        {
            
        }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}