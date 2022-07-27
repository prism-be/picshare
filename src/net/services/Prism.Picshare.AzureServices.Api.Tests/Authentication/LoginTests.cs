// -----------------------------------------------------------------------
//  <copyright file = "LoginTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Prism.Picshare.AzureServices.Api.Authentication;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Authentication;

public class LoginTests
{

    [Fact]
    public async Task Login_Invalid_Credentials()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResultCodes.InvalidCredentials);

        var (requestData, context) = AzureFunctionContext.Generate(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Act
        var controller = new Login(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Not_Found()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResultCodes.Ok);

        var (requestData, context) = AzureFunctionContext.Generate(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Act
        var controller = new Login(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_Ok()
    {
        // Arrange
        var accessToken = Guid.NewGuid().ToString();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<AuthenticationRequest>(), default)).ReturnsAsync(ResultCodes.Ok);
        mediator.Setup(x => x.Send(It.IsAny<GenerateTokenRequest>(), default)).ReturnsAsync(new Token
        {
            AccessToken = accessToken
        });

        var (requestData, context) = AzureFunctionContext.Generate(new AuthenticationRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

        // Act
        var controller = new Login(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = result.GetBody<Token>();
        token?.AccessToken.Should().Be(accessToken);
    }
}