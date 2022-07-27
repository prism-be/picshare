// -----------------------------------------------------------------------
//  <copyright file = "FlowTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Pictures;

public class FlowTests
{
    [Fact]
    public async Task GetFlow_Ok()
    {
        // Arrange
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, It.IsAny<string>(), It.IsAny<string>(), new Flow());
        var (requestData, context) = AzureFunctionContext.Generate();
        
        // Act
        var controller = new Api.Pictures.Flow(storeClient.Object);
        var flow = await controller.Run(requestData.Object, context.Object);

        // Assert
        flow.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}