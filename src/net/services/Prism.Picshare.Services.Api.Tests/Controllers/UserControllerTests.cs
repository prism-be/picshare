// -----------------------------------------------------------------------
//  <copyright file = "UserControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Api.Controllers;

namespace Prism.Picshare.Services.Api.Tests.Controllers;

public class UserControllerTests
{
    [Fact]
    public void Check_Ok()
    {
        // Arrange
        var usercontext = UserContextMock.Generate();

        // Act
        var controller = new UserController(usercontext.Object);
        var result = controller.Check();

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public void Check_Unauthorized()
    {
        // Arrange
        var usercontext = UserContextMock.Generate(isAuthenticated: false);

        // Act
        var controller = new UserController(usercontext.Object);
        var result = controller.Check();

        // Assert
        result.Should().BeAssignableTo<UnauthorizedObjectResult>();
    }
}