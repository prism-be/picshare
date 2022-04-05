// -----------------------------------------------------------------------
//  <copyright file="DatabaseConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data;

public class DatabaseConfiguration
{
    public DatabaseConfiguration(string databaseDirectory, string databasePassword)
    {
        DatabaseDirectory = databaseDirectory;
        DatabasePassword = databasePassword;
    }

    public string DatabaseDirectory { get; init; }

    public string DatabasePassword { get; init; }

    public IDatabaseResolver? DatabaseResolver { get; private set; }

    public void WithDatabaseResolver(IDatabaseResolver? resolver)
    {
        DatabaseResolver = resolver;
    }
}