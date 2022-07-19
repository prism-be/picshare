// -----------------------------------------------------------------------
//  <copyright file = "InsightsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Authentication.Controllers.Api;

namespace Prism.Picshare.Services.Authentication.Tests.Controllers.Api;

public class InsightsControllerTests
{

    [Fact]
    public void GetConfig_EmptyConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", string.Empty);

        // Act
        var controller = new InsightsController();
        var config = controller.GetConfiguration();

        // Assert
        config.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public void GetConfig_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
            "InstrumentationKey=158da90e-21a9-406c-918b-79ccad7d5364;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/");

        // Act
        var controller = new InsightsController();
        var config = controller.GetConfiguration();

        // Assert
        config.Should().BeAssignableTo<OkObjectResult>();
    }
}