// -----------------------------------------------------------------------
//  <copyright file="FakeCosmosClient.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeCosmosClient: CosmosClient
{
    private readonly Container _container;

    public FakeCosmosClient(Container container)
    {
        _container = container;
    }

    public override Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(string id, int? throughput = null, RequestOptions requestOptions = null!, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(new FakeDatabaseResponse(_container) as DatabaseResponse);
    }
}