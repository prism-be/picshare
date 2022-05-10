// -----------------------------------------------------------------------
//  <copyright file="OrganisationRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.CosmosDB;

public class OrganisationRepository : IOrganisationRepository
{
    private readonly CosmosClient _cosmosClient;

    public OrganisationRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<int> CreateOrganisationAsync(Organisation organisation)
    {
        var container = await EnsureContainersAsync();

        var result = await container.CreateItemAsync(organisation);

        return (int)result.StatusCode;
    }

    private async Task<Container> EnsureContainersAsync()
    {
        var db = await _cosmosClient.CreateDatabaseIfNotExistsAsync(nameof(Databases.General));

        var organisationContainer = new ContainerProperties
        {
            Id = Databases.General.Organisations,
            PartitionKeyPath = "/id",
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = false, IndexingMode = IndexingMode.None
            }
        };

        organisationContainer.IndexingPolicy.ExcludedPaths.Clear();
        organisationContainer.IndexingPolicy.IncludedPaths.Clear();

        var organisations = await db.Database.CreateContainerIfNotExistsAsync(organisationContainer);

        return organisations.Container;
    }
}