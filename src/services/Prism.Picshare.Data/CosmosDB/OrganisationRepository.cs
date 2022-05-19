// -----------------------------------------------------------------------
//  <copyright file="OrganisationRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.CosmosDB;

public class OrganisationRepository : IOrganisationRepository
{
    public const string Database = "picshare";
    public const string Container = "organisations";
    
    private readonly CosmosClient _cosmosClient;

    public OrganisationRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<int> CreateOrganisationAsync(Organisation organisation)
    {
        var container = _cosmosClient.GetDatabase(Database).GetContainer(Container);

        var result = await container.CreateItemAsync(organisation);

        return (int)result.StatusCode;
    }

    public async Task<Organisation?> GetOrganisationAsync(Guid id)
    {
        var container = _cosmosClient.GetDatabase(Database).GetContainer(Container);
        return await container.ReadItemAsync<Organisation>(id.ToString(), new PartitionKey(id.ToString()));
    }
}