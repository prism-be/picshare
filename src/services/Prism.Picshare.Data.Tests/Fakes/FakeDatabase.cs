// -----------------------------------------------------------------------
//  <copyright file="FakeDatabase.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeDatabase : Database
{
    private Container _container;

    public FakeDatabase(Container container)
    {
        _container = container;
    }

    public override Task<DatabaseResponse> ReadAsync(RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<DatabaseResponse> DeleteAsync(RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(new FakeContainerResponse(_container) as ContainerResponse);
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties, ThroughputProperties throughputProperties, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ResponseMessage> ReadStreamAsync(RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ResponseMessage> DeleteStreamAsync(RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Container GetContainer(string id)
    {
        throw new System.NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerAsync(string id, string partitionKeyPath, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(ContainerProperties containerProperties, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult<ContainerResponse>(new FakeContainerResponse(_container));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(string id, string partitionKeyPath, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override User GetUser(string id)
    {
        throw new System.NotImplementedException();
    }

    public override Task<UserResponse> CreateUserAsync(string id, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override Task<UserResponse> UpsertUserAsync(string id, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator GetContainerQueryStreamIterator(QueryDefinition queryDefinition, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(string queryText = null!, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator GetContainerQueryStreamIterator(string queryText = null!, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(string queryText = null!, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(QueryDefinition queryDefinition, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath)
    {
        throw new System.NotImplementedException();
    }

    public override ClientEncryptionKey GetClientEncryptionKey(string id)
    {
        throw new System.NotImplementedException();
    }

    public override FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(QueryDefinition queryDefinition, string continuationToken = null!, QueryRequestOptions requestOptions = null!)
    {
        throw new System.NotImplementedException();
    }

    public override Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(ClientEncryptionKeyProperties clientEncryptionKeyProperties, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new System.NotImplementedException();
    }

    public override string Id { get; }
    public override CosmosClient Client { get; }
}