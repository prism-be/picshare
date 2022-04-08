// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Data.Exceptions;

namespace Prism.Picshare.Data.LiteDB;

public static class ServiceCollectionExtensions
{
    public static void UseLiteDbStorage(this IServiceCollection services)
    {
        var databasesDirectory = Environment.GetEnvironmentVariable("PICSHARE_DB_DIRECTORY");
        var databasePassword = Environment.GetEnvironmentVariable("PICSHARE_DB_PASSWORD");

        if (string.IsNullOrWhiteSpace(databasesDirectory))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_DIRECTORY");
        }

        if (string.IsNullOrWhiteSpace(databasePassword))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_PASSWORD");
        }

        var configuration = new DatabaseConfiguration(databasesDirectory, databasePassword);
        services.AddSingleton(configuration);

        services.AddScoped<IDatabaseResolver, DatabaseResolver>();
    }
}