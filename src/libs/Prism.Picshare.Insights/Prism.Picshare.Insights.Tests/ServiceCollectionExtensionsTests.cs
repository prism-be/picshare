// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Prism.Picshare.Insights.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInsights_Ok()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddInsights();

        // Assert
        services.Should().Contain(x => x.ImplementationType == typeof(PicshareTelemetryInitializer));
    }
}