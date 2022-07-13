// -----------------------------------------------------------------------
//  <copyright file = "ThumbsControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;
using Prism.Picshare.Services.Pictures.Controllers.Api;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Api;

public class ThumbsControllerTests
{

    [Fact]
    public async Task GetThumbs_Not_Found()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<ThumbsController>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var daprClient = new Mock<DaprClient>();

        // Act
        var controller = new ThumbsController(logger.Object, userContextAccessor.Object, daprClient.Object);
        var thumb = await controller.GetThumbs(organisationId, Guid.NewGuid(), 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task GetThumbs_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<ThumbsController>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.InvokeBindingAsync(It.IsAny<BindingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BindingResponse(new BindingRequest(Stores.Data, "get"), Samples.SmallImage, new Dictionary<string, string>()));

        // Act
        var controller = new ThumbsController(logger.Object, userContextAccessor.Object, daprClient.Object);
        var thumb = await controller.GetThumbs(organisationId, Guid.NewGuid(), 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<FileResult>();
    }

    [Fact]
    public async Task GetThumbs_Unauthorized()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<ThumbsController>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var daprClient = new Mock<DaprClient>();

        // Act
        var controller = new ThumbsController(logger.Object, userContextAccessor.Object, daprClient.Object);
        var thumb = await controller.GetThumbs(Guid.NewGuid(), Guid.NewGuid(), 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<NotFoundResult>();
    }
}