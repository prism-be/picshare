// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Insights.Tests.Fakes;
using Xunit;

namespace Prism.Picshare.Insights.Tests;

public class LoggingBuilderExtensionsTests
{
    [Fact]
    public void AddInsights_Ok()
    {
        // Arrange
        var builder = new DummyLoggingBuilder();

        // Act
        builder.AddInsights();

        // Assert
        builder.Services.Should().Contain(x => x.ImplementationType == typeof(PicshareTelemetryInitializer));
    }
}