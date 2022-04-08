﻿// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Data.Exceptions;
using Xunit;

namespace Prism.Picshare.Data.LiteDB.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void UseLiteDb_OK()
    {
        // Arrange
        var password = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", Path.GetTempPath());
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", password);

        // Act
        var services = new ServiceCollection();
        services.UseLiteDbStorage();

        // Assert
        Assert.Equal(1, services.Count(x => x.ServiceType == typeof(DatabaseConfiguration)));
        Assert.Equal(1, services.Count(x => x.ImplementationType == typeof(DatabaseResolver)));

        var config = services.Single(x => x.ServiceType == typeof(DatabaseConfiguration)).ImplementationInstance as DatabaseConfiguration;

        Assert.NotNull(config);
        Assert.Equal(Path.GetTempPath(), config!.DatabaseDirectory);
        Assert.Equal(password, config.DatabasePassword);
    }

    [Fact]
    public void UseLiteDb_NO_DB_DIRECTORY()
    {
        // Arrange
        var password = Guid.NewGuid().ToString();
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", null);
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", password);

        // Act
        var services = new ServiceCollection();
        var ex = Assert.Throws<DatabaseConfigurationException>(() => services.UseLiteDbStorage());

        // Assert
        Assert.Equal("Application cannot start because of missing variable: PICSHARE_DB_DIRECTORY", ex.Message);
    }

    [Fact]
    public void UseLiteDb_NO_DB_PASSWORD()
    {
        // Arrange
        Environment.SetEnvironmentVariable("PICSHARE_DB_DIRECTORY", Path.GetTempPath());
        Environment.SetEnvironmentVariable("PICSHARE_DB_PASSWORD", null);

        // Act
        var services = new ServiceCollection();
        var ex = Assert.Throws<DatabaseConfigurationException>(() => services.UseLiteDbStorage());

        // Assert
        Assert.Equal("Application cannot start because of missing variable: PICSHARE_DB_PASSWORD", ex.Message);
    }
}