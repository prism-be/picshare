// -----------------------------------------------------------------------
//  <copyright file = "InsightsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Config;

public class InsightsControllerTests
{

    [Fact]
    public void GetConfig_EmptyConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", string.Empty);
        var requestData = new Mock<HttpRequestData>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new AzureServices.Api.Config.Insights();
        var config = controller.Run(requestData.Object, context.Object);

        // Assert
        config.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public void GetConfig_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
            "InstrumentationKey=158da90e-21a9-406c-918b-79ccad7d5364;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/");
        var requestData = new Mock<HttpRequestData>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new AzureServices.Api.Config.Insights();
        var config = controller.Run(requestData.Object, context.Object);

        // Assert
        config.Should().BeAssignableTo<OkObjectResult>();
    }
}