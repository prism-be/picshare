// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.AspNetCore;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPicshareDependencies_Ok()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPicshareDependencies();

        // Assert
        services.Should().NotBeEmpty();
    }
}