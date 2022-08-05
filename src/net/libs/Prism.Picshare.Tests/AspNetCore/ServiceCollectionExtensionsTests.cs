// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.AspNetCore;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPicshare_Ok()
    {
        // Arrange
        var services = new ServiceCollection();
        Environment.SetEnvironmentVariable("JWT_PUBLIC_KEY", Guid.NewGuid().ToString());
        Environment.SetEnvironmentVariable("JWT_PRIVATE_KEY", Guid.NewGuid().ToString());
        Environment.SetEnvironmentVariable("COSMOS_CONNECTION_STRING", "AccountEndpoint=https://abcd.documents.azure.com:443/;AccountKey=abcd;");
        Environment.SetEnvironmentVariable("REDIS_CONNECTION_STRING", Guid.NewGuid().ToString());
        
        // Act
        var returnedServices = services.AddDisconnectedPicshareServices();

        // Assert
        returnedServices.Should().BeEquivalentTo(services);
        services.Count.Should().BePositive();
    }
}