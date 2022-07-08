// -----------------------------------------------------------------------
//  <copyright file = "UserControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Authentication.Controllers.Api;

namespace Prism.Picshare.Services.Authentication.Tests.Controllers.Api;

public class UserControllerTests
{
    [Fact]
    public void Check_Ok()
    {
        // Arrange
        var userContextAcessor = new Mock<IUserContextAccessor>();
        userContextAcessor.Setup(x => x.Name).Returns("Hello World !");
        userContextAcessor.Setup(x => x.IsAuthenticated).Returns(true);
        var controller = new UserController(userContextAcessor.Object);

        // Act
        var result = controller.Check();

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public void Check_Unauthorized()
    {
        // Arrange
        var userContextAcessor = new Mock<IUserContextAccessor>();
        userContextAcessor.Setup(x => x.IsAuthenticated).Returns(false);
        var controller = new UserController(userContextAcessor.Object);

        // Act
        var result = controller.Check();

        // Assert
        result.Should().BeAssignableTo<UnauthorizedObjectResult>();
    }
}