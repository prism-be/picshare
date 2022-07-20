// -----------------------------------------------------------------------
//  <copyright file = "PicturesControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Api;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Api;

public class PicturesControllerTests
{
    [Fact]
    public async Task GetPicture_Forbidden()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.HasAccess(organisationId)).Returns(false);
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new PicturesController(mediator.Object, userContextAccessor.Object);
        var result = await controller.GetPicture(organisationId, Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Never());
    }

    [Fact]
    public async Task GetPicture_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.HasAccess(organisationId)).Returns(true);
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new PicturesController(mediator.Object, userContextAccessor.Object);
        var result = await controller.GetPicture(organisationId, Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Once());
    }
}