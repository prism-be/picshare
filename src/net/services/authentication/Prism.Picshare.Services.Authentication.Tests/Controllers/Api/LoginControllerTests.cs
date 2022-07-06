// -----------------------------------------------------------------------
//  <copyright file = "LoginControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Commands;
using Prism.Picshare.Services.Authentication.Configuration;
using Prism.Picshare.Services.Authentication.Controllers.Api;

namespace Prism.Picshare.Services.Authentication.Tests.Controllers.Api;

public class LoginControllerTests
{

    [Fact]
    public async Task Login_Invalid_Credentials()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResponseCodes.InvalidCredentials);

        // Act
        var controller = new LoginController(mediator.Object);
        var result = await controller.Login(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.Should().BeAssignableTo<UnauthorizedResult>();
    }

    [Fact]
    public async Task Login_Not_Found()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResponseCodes.Ok);

        // Act
        var controller = new LoginController(mediator.Object);
        var result = await controller.Login(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.Should().BeAssignableTo<BadRequestResult>();
    }

    [Fact]
    public async Task Login_Ok()
    {
        // Arrange
        var accessToken = Guid.NewGuid().ToString();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResponseCodes.Ok);
        mediator.Setup(x => x.Send(It.IsAny<GenerateTokenRequest>(), default)).ReturnsAsync(new Token
        {
            AccessToken = accessToken
        });

        // Act
        var controller = new LoginController(mediator.Object);
        var result = await controller.Login(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        var token = (Token)((OkObjectResult)result).Value!;
        token.AccessToken.Should().Be(accessToken);
    }

    [Theory]
    [InlineData(ResponseCodes.Ok, typeof(NoContentResult))]
    [InlineData(ResponseCodes.ExistingOrganisation, typeof(ConflictObjectResult))]
    [InlineData(ResponseCodes.ExistingUsername, typeof(ConflictObjectResult))]
    public async Task Register(ResponseCodes code, Type responseType)
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<RegisterAccountRequest>(), default)).ReturnsAsync(code);

        // Act
        var controller = new LoginController(mediator.Object);
        var result = await controller.Register(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()));

        // Assert
        result.Should().BeAssignableTo(responseType);
    }
}