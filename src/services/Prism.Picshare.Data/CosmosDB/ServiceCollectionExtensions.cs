// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;

namespace Prism.Picshare.Data.CosmosDB;

public static class ServiceCollectionExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void UseCosmosDB(this IServiceCollection services)
    {
        var cosmosDbConnectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

        services.AddSingleton(_ =>
        {
            var cosmosClientBuilder = new CosmosClientBuilder(cosmosDbConnectionString);

            return cosmosClientBuilder.WithConnectionModeDirect()
                .WithBulkExecution(true)
                .Build();
        });
        
        services.AddScoped<IOrganisationRepository, OrganisationRepository>();
    }
}