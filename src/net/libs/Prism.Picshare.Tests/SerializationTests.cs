﻿// -----------------------------------------------------------------------
//  <copyright file = "SerializationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
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
    public void Album_Ok()
    {
        // Arrange
        var source = new Album
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            OrganisationId = Guid.NewGuid()
        };

        CheckSerialization(source);
    }

    [Fact]
    public void Credentials_Ok()
    {
        // Arrange
        var source = new Credentials
        {
            Id = Guid.NewGuid(),
            Login = Guid.NewGuid().ToString(),
            PasswordHash = Guid.NewGuid().ToString()
        };

        CheckSerialization(source);
    }

    [Fact]
    public void EmailAction_Ok()
    {
        // Arrange
        var source = new EmailAction();

        CheckSerialization(source);
    }

    [Fact]
    public void Organisation_Ok()
    {
        // Arrange
        var source = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString()
        };

        source.Users.Add(Guid.NewGuid());
        source.Users.Add(Guid.NewGuid());
        source.Users.Add(Guid.NewGuid());

        CheckSerialization(source);
    }

    [Fact]
    public void PhotoboothPicture_Ok()
    {
        // Arrange
        var source = new PhotoboothPicture
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            OriginalFileName = Guid.NewGuid().ToString()
        };

        CheckSerialization(source);
    }

    [Fact]
    public void Picture_Ok()
    {
        // Arrange
        var source = new Picture
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            Source = PictureSource.Upload,
            CreationDate = DateTime.Today,
            Views = 42,
            Name = Guid.NewGuid().ToString()
        };

        CheckSerialization(source);
    }

    [Fact]
    public void User_Ok()
    {
        // Arrange
        var source = new User
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            Email = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            EmailValidated = true
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