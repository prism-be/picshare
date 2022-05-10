// -----------------------------------------------------------------------
//  <copyright file="Startup.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Prism.Picshare;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var cosmosDbConnectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTION_STRING");

        builder.Services.AddSingleton(_ =>
        {
            var cosmosClientBuilder = new CosmosClientBuilder(cosmosDbConnectionString);

            return cosmosClientBuilder.WithConnectionModeDirect()
                .WithBulkExecution(true)
                .Build();
        });
    }
}