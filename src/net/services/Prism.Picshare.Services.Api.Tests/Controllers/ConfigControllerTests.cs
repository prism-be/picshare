// -----------------------------------------------------------------------
//  <copyright file = "ConfigControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Api.Controllers;

namespace Prism.Picshare.Services.Api.Tests.Controllers;

public class ConfigControllerTests
{
    [Fact]
    public void Insights_EmptyConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", string.Empty);

        // Act
        var controller = new ConfigController();
        var result = controller.Insights();

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public void Insights_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
            "InstrumentationKey=158da90e-21a9-406c-918b-79ccad7d5364;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/");

        // Act
        var controller = new ConfigController();
        var result = controller.Insights();

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }
}