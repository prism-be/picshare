// -----------------------------------------------------------------------
//  <copyright file = "InsightsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Config;

public class InsightsControllerTests
{

    [Fact]
    public async Task GetConfig_EmptyConnectionString()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", string.Empty);
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new AzureServices.Api.Config.Insights();
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetConfig_Ok()
    {
        // Arrange
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING",
            "InstrumentationKey=158da90e-21a9-406c-918b-79ccad7d5364;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/");
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new AzureServices.Api.Config.Insights();
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}