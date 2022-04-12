// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Security.Exceptions;
using Xunit;

namespace Prism.Picshare.Security.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddJwtAuthentication_NoAudience()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var ex = Assert.Throws<JwtConfigurationException>(() => services.AddJwtAuthentication(config =>
        {
            config.Key = Guid.NewGuid().ToString();
            config.Issuer = Guid.NewGuid().ToString();
        }));

        // Assert
        Assert.Equal("Application cannot start because of missing variable: JwtConfiguration.Audience", ex.Message);
    }

    [Fact]
    public void AddJwtAuthentication_NoIssuer()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var ex = Assert.Throws<JwtConfigurationException>(() => services.AddJwtAuthentication(config =>
        {
            config.Key = Guid.NewGuid().ToString();
            config.Audience = Guid.NewGuid().ToString();
        }));

        // Assert
        Assert.Equal("Application cannot start because of missing variable: JwtConfiguration.Issuer", ex.Message);
    }

    [Fact]
    public void AddJwtAuthentication_NoKey()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var ex = Assert.Throws<JwtConfigurationException>(() => services.AddJwtAuthentication(config =>
        {
            config.Audience = Guid.NewGuid().ToString();
            config.Issuer = Guid.NewGuid().ToString();
        }));

        // Assert
        Assert.Equal("Application cannot start because of missing variable: JwtConfiguration.Key", ex.Message);
    }

    [Fact]
    public void AddJwtAuthentication_Ok()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddJwtAuthentication(config =>
        {
            config.Key = Guid.NewGuid().ToString();
            config.Audience = Guid.NewGuid().ToString();
            config.Issuer = Guid.NewGuid().ToString();
        });

        // Assert
        Assert.Equal(1, services.Count(x => x.ServiceType == typeof(JwtConfiguration)));
    }
}