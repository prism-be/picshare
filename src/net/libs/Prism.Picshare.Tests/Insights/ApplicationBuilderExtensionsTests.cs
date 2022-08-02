// -----------------------------------------------------------------------
//  <copyright file = "ApplicationBuilderExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Moq;
using Prism.Picshare.Insights;
using Xunit;

namespace Prism.Picshare.Tests.Insights;

public class ApplicationBuilderExtensionsTests
{
    [Fact]
    public void UseForemLogger_Ok()
    {
        // Arrange
        var app = new Mock<IApplicationBuilder>();

        // Act
        app.Object.UseExceptionLogger();

        // Assert
        Assert.NotNull(app);
    }
}