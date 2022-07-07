// -----------------------------------------------------------------------
//  <copyright file = "UserControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Services.Authentication.Controllers.Api;

namespace Prism.Picshare.Services.Authentication.Tests.Controllers.Api;

public class UserControllerTests
{
    [Fact]
    public void Check_Ok()
    {
        // Arrange
        var controller = new UserController();
        
        // Act
        var result = controller.Check();

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
    }
}