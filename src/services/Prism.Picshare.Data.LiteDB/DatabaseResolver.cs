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
    private readonly DatabaseConfiguration _databaseConfiguration;

    public DatabaseResolver(DatabaseConfiguration databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration;
    }

    public IDatabase GetDatabase(string organisation, string databaseType)
    {
        if (string.IsNullOrWhiteSpace(_databaseConfiguration.DatabaseDirectory))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_DIRECTORY");
        }

        if (string.IsNullOrWhiteSpace(_databaseConfiguration.DatabasePassword))
        {
            throw new DatabaseConfigurationException("Application cannot start because of missing variable: PICSHARE_DB_PASSWORD");
        }

        var databasePath = Path.Combine(_databaseConfiguration.DatabaseDirectory, $"{organisation}-{databaseType}.db");
        var connectionString = $"Filename={databasePath};Password={_databaseConfiguration.DatabasePassword};";

        var liteDatabase = new LiteDatabase(connectionString);

        return new Database(liteDatabase);
    }
}