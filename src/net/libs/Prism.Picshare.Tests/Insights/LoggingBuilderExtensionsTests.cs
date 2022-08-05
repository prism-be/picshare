// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Prism.Picshare.Insights;
using Prism.Picshare.Tests.Insights.Fakes;
using Xunit;

namespace Prism.Picshare.Tests.Insights;

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
        builder.Services.Count.Should().BePositive();
    }
}