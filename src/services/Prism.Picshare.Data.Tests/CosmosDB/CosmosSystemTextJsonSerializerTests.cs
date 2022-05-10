// -----------------------------------------------------------------------
//  <copyright file="CosmosSystemTextJsonSerializerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using Prism.Picshare.Data.CosmosDB;
using Xunit;

namespace Prism.Picshare.Data.Tests.CosmosDB;

public class CosmosSystemTextJsonSerializerTests
{
    [Fact]
    public void CosmosSystemTextJsonSerializer_Ok()
    {
        var organisation = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = "Unit Tests"
        };

        var serializer = new CosmosSystemTextJsonSerializer(new JsonSerializerOptions());
        var serialized = serializer.ToStream(organisation);
        var deserialized = serializer.FromStream<Organisation>(serialized);
        
        Assert.Equal(organisation.Id, deserialized.Id);
        Assert.Equal(organisation.Name, deserialized.Name);
    }
}