// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Prism.Picshare.Data.CosmosDB;
using Prism.Picshare.Data.Tests.Fakes;
using Xunit;

namespace Prism.Picshare.Data.Tests.CosmosDB;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void UseCosmosDB_Ok()
    {
        var cosmosClient = new FakeCosmosClient(Mock.Of<Container>()) as CosmosClient;
        var services = new ServiceCollection();
        services.UseCosmosDB(cosmosClient);
        
        Assert.NotNull(services.FirstOrDefault(x => x.ServiceType == typeof(CosmosClient)));
    }
}