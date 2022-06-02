﻿// -----------------------------------------------------------------------
//  <copyright file="SerializationTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using FluentAssertions;
using Prism.Picshare.Domain;
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

        CheckSerialization(source);
    }

    private static void CheckSerialization<T>(T source)
    {
        // Act
        var destination = SerializeAndDeserialize(source);

        // Assert
        destination.Should().BeEquivalentTo(source);
    }

    [Fact]
    public void Picture_Ok()
    {
        // Arrange
        var source = new Picture
        {
            Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid(), Source = PictureSource.Upload, CreationDate = DateTime.Today
        };

        CheckSerialization(source);
    }

    private static T? SerializeAndDeserialize<T>(T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json);
    }
}