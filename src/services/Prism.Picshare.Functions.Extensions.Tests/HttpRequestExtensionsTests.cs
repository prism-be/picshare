// -----------------------------------------------------------------------
//  <copyright file="HttpRequestExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Prism.Picshare.Functions.Extensions.Tests;

public class HttpRequestExtensionsTests
{
    [Fact]
    public void nameGetOrganisationId_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.Request.Headers.Add("X-OrganisationId", organisationId.ToString());

        // Act
        var foundOrganisationId = context.Request.GetOrganisationId();

        // Assert
        foundOrganisationId.Should().Be(organisationId);
    }

    [Fact]
    public void GetOrganisationId_NotPresent()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Act

        var ex = Assert.Throws<UnauthorizedAccessException>(() => context.Request.GetOrganisationId());

        // Assert
        ex.Message.Should().Be("The organisation id cannot be found");
    }

    [Fact]
    public void GetOrganisationId_InvalidFormat()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers.Add("X-OrganisationId", "42");

        // Act

        var ex = Assert.Throws<UnauthorizedAccessException>(() => context.Request.GetOrganisationId());

        // Assert
        ex.Message.Should().Be("The organisation id is not well formatted");
    }
}