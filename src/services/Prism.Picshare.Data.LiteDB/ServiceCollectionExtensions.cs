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
    public static void UseLiteDbStorage(this IServiceCollection services, Action<DatabaseConfiguration> config)
    {
        var configuration = new DatabaseConfiguration();
        config(configuration);

        if (string.IsNullOrWhiteSpace(configuration.DatabaseDirectory))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_DIRECTORY");
        }

        if (string.IsNullOrWhiteSpace(configuration.DatabasePassword))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_PASSWORD");
        }

        services.AddSingleton(configuration);

        services.AddScoped<IDatabaseResolver, DatabaseResolver>();
    }
}