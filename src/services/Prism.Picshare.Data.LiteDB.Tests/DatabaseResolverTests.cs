// -----------------------------------------------------------------------
//  <copyright file="DatabaseResolverTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using Prism.Picshare.Data.Exceptions;
using Xunit;

namespace Prism.Picshare.Data.LiteDB.Tests;

public class DatabaseResolverTests
{
    [Fact]
    public void GetDatabase_No_DB_DIRECTORY()
    {
        // Arrange
        var organisation = Guid.NewGuid().ToString();
        var type = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", null);
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", password);

        var databaseResolver = new DatabaseResolver();

        // Act
        var ex = Assert.Throws<DatabaseConfigurationException>(() => databaseResolver.GetDatabase(organisation, type));

        // Assert
        Assert.Equal("Application cannot start because of missing variable: PICSHARE_DB_DIRECTORY", ex.Message);
    }

    [Fact]
    public void GetDatabase_No_DB_DIRECTORY_PASSWORD()
    {
        // Arrange
        var organisation = Guid.NewGuid().ToString();
        var type = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", Path.GetTempPath());
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", null);

        var databaseResolver = new DatabaseResolver();

        // Act
        var ex = Assert.Throws<DatabaseConfigurationException>(() => databaseResolver.GetDatabase(organisation, type));

        // Assert
        Assert.Equal("Application cannot start because of missing variable: PICSHARE_DB_PASSWORD", ex.Message);
    }

    [Fact]
    public void GetDatabase_Ok()
    {
        // Arrange
        var organisation = Guid.NewGuid().ToString();
        var type = Guid.NewGuid().ToString();
        var password = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", Path.GetTempPath());
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", password);

        var databaseResolver = new DatabaseResolver();

        // Act
        using var db = databaseResolver.GetDatabase(organisation, type);

        // Assert
        Assert.NotNull(db);
    }
}