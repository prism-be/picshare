// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Picshare.Data.CosmosDB;

public static class ServiceCollectionExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void UseCosmosDB(this IServiceCollection services, CosmosClient? cosmosClient = null)
    {
        var cosmosDbConnectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

        services.AddSingleton(_ =>
        {
            var cosmosSystemTextJsonSerializer = new CosmosSystemTextJsonSerializer(
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

            var cosmosClientBuilder = new CosmosClientBuilder(cosmosDbConnectionString);

            return cosmosClientBuilder.WithConnectionModeDirect()
                .WithBulkExecution(true)
                .WithCustomSerializer(cosmosSystemTextJsonSerializer)
                .Build();
        });

        services.AddScoped<IOrganisationRepository, OrganisationRepository>();
        services.AddScoped<IPictureRepository, PictureRepository>();

        cosmosClient ??= new CosmosClient(cosmosDbConnectionString);

        InitializeCosmosDb(cosmosClient);
    }

    private static void InitializeCosmosDb(CosmosClient cosmosClient)
    {
        var task = async () =>
        {
            await InitializeOrganisationsContainer(cosmosClient);
            await InitializePicturesContainer(cosmosClient);
        };

        task.Invoke().Wait();
    }

    private static async Task InitializePicturesContainer(CosmosClient cosmosClient)
    {
        var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(PictureRepository.Database);

        var organisationContainer = new ContainerProperties
        {
            Id = PictureRepository.Container,
            PartitionKeyPath = "/organisationId",
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = true
            }
        };

        await db.Database.CreateContainerIfNotExistsAsync(organisationContainer);
    }

    private static async Task InitializeOrganisationsContainer(CosmosClient cosmosClient)
    {
        var db = await cosmosClient.CreateDatabaseIfNotExistsAsync(OrganisationRepository.Database);

        var organisationContainer = new ContainerProperties
        {
            Id = OrganisationRepository.Container,
            PartitionKeyPath = "/id",
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = false, IndexingMode = IndexingMode.None
            }
        };

        organisationContainer.IndexingPolicy.ExcludedPaths.Clear();
        organisationContainer.IndexingPolicy.IncludedPaths.Clear();

        await db.Database.CreateContainerIfNotExistsAsync(organisationContainer);
    }
}