// -----------------------------------------------------------------------
//  <copyright file="SerializationTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Prism.Picshare.Tests;

public class SerializationTests
{
    [Fact]
    public void Organisation_Ok()
    {
        // Arrange
        var source = new Organisation
        {
            Id = Guid.NewGuid(), Name = Guid.NewGuid().ToString()
        };

        // Act
        var destination = this.SerializeAndDeserialize(source);

        // Assert
        destination.Should().BeEquivalentTo(source);
    }

    [Fact]
    public void Picture_Ok()
    {
        // Arrange
        var source = new Picture
        {
            Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid(), Source = PictureSource.Upload
        };

        // Act
        var destination = this.SerializeAndDeserialize(source);

        // Assert
        destination.Should().BeEquivalentTo(source);
    }

    private T? SerializeAndDeserialize<T>(T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }
}