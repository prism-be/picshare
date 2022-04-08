// -----------------------------------------------------------------------
//  <copyright file="DatabaseConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data.LiteDB;

public class DatabaseConfiguration
{
    public DatabaseConfiguration(string databaseDirectory, string databasePassword)
    {
        DatabaseDirectory = databaseDirectory;
        DatabasePassword = databasePassword;
    }

    public string DatabaseDirectory { get; init; }

    public string DatabasePassword { get; init; }
}