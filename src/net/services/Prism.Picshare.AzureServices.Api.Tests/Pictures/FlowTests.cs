// -----------------------------------------------------------------------
//  <copyright file = "FlowTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, string.Empty, organisationId.ToString(), new Flow
        {
            Id = organisationId
        });
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, new Authorizations
        {
            Id = userId
        });
        var (requestData, context) = AzureFunctionContext.Generate(organisationId: organisationId, userId: userId);

        // Act
        var controller = new Api.Pictures.Flow(storeClient.Object);
        var flow = await controller.Run(requestData.Object, context.Object);

        // Assert
        flow.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}