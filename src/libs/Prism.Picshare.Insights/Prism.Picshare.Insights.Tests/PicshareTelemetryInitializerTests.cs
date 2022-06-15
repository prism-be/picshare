// -----------------------------------------------------------------------
//  <copyright file = "PicshareTelemetryInitializerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace Prism.Picshare.Insights.Tests;

public class PicshareTelemetryInitializerTests
{
    [Fact]
    public void Initialize_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_ROLE_NAME", "AIROLE");
        var telemetry = new TraceTelemetry();

        // Act
        var initializer = new PicshareTelemetryInitializer();
        initializer.Initialize(telemetry);

        // Assert
        telemetry.Context.Cloud.RoleName.Should().Be("AIROLE");
    }
}