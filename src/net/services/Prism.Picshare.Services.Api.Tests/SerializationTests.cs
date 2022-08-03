// -----------------------------------------------------------------------
//  <copyright file = "SerializationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using FluentAssertions;
using Prism.Picshare.Services.Api.Contracts;

namespace Prism.Picshare.Services.Api.Tests;

public class SerializationTests
{
    [Fact]
    public void User_Ok()
    {
        // Arrange
        var source = new UserAuthentication
        {
            Name = Guid.NewGuid().ToString(),
            Authenticated = true,
            Organisation = Guid.NewGuid()
        };

        CheckSerialization(source);
    }

    private static void CheckSerialization<T>(T source)
    {
        // Act
        var destination = SerializeAndDeserialize(source);

        // Assert
        destination.Should().BeEquivalentTo(source);
    }

    private static T? SerializeAndDeserialize<T>(T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }
}