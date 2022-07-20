// -----------------------------------------------------------------------
//  <copyright file = "FlowControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Controllers.Api;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Api;

public class FlowControllerTests
{
    [Fact]
    public async Task GetFlow_Ok()
    {
        // Arrange
        var userContextAccessor = new Mock<IUserContextAccessor>();

        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, It.IsAny<string>(), new Flow());

        // Act
        var controller = new FlowController(userContextAccessor.Object, storeClient.Object);
        var flow = await controller.GetFlow();

        // Assert
        flow.Should().BeAssignableTo<OkObjectResult>();
    }
}