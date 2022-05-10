// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Data.CosmosDB;
using Xunit;

namespace Prism.Picshare.Data.Tests.CosmosDB;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void UseCosmosDB_Ok()
    {
        var services = new ServiceCollection();
        services.UseCosmosDB();
        
        Assert.NotNull(services.FirstOrDefault(x => x.ServiceType == typeof(CosmosClient)));
    }
}