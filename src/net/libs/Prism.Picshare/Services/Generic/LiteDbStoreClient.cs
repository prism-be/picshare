// -----------------------------------------------------------------------
//  <copyright file = "LiteDbStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using LiteDB;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Prism.Picshare.Services.Generic;

public class LiteDbStoreClient : BaseMutableStoreClient
{
    public LiteDbStoreClient(RedisLocker locker) : base(locker)
    {
    }

    public override Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class
    {
        using var db = new LiteDatabase(GetDatabaseConnectionString(organisationId));
        var collection = db.GetCollection<DataStorage>(store);
        var data = collection.FindById(id)?.Data;

        if (data == null)
        {
            return Task.FromResult((T?)null);
        }

        return Task.FromResult(JsonSerializer.Deserialize<T>(data));
    }

    public override Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default)
    {
        using var db = new LiteDatabase(GetDatabaseConnectionString(organisationId));
        var collection = db.GetCollection<DataStorage>(store);
        collection.Upsert(new DataStorage
        {
            Id = id,
            Data = JsonSerializer.Serialize(data)
        });
        collection.EnsureIndex(x => x.Id);
        return Task.CompletedTask;
    }

    private static string GetDatabaseConnectionString(string organisationId)
    {
        var baseConnectionString = "Connection=shared;Filename=";

        if (string.IsNullOrWhiteSpace(organisationId))
        {
            return baseConnectionString + Path.Combine(EnvironmentConfiguration.GetMandatoryConfiguration("LITE_DB_DIRECTORY"), $"{Guid.Empty}.db");
        }

        return baseConnectionString + Path.Combine(EnvironmentConfiguration.GetMandatoryConfiguration("LITE_DB_DIRECTORY"), $"{organisationId}.db");
    }

    private sealed class DataStorage
    {

        [JsonPropertyName("data")]
        public string? Data { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}