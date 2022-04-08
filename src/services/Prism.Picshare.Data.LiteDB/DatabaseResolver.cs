// -----------------------------------------------------------------------
//  <copyright file="DatabaseResolver.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using LiteDB;
using Prism.Picshare.Data.Exceptions;

namespace Prism.Picshare.Data.LiteDB;

public class DatabaseResolver : IDatabaseResolver
{
    public IDatabase GetDatabase(string organisation, string databaseType)
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

        var databasePath = Path.Combine(databasesDirectory, $"{organisation}-{databaseType}.db");
        var connectionString = $"Filename={databasePath};Password={databasePassword};";

        var liteDatabase = new LiteDatabase(connectionString);

        return new Database(liteDatabase);
    }
}