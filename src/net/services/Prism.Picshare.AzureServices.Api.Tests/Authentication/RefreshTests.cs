// -----------------------------------------------------------------------
//  <copyright file = "RefreshTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.AzureServices.Api.Authentication;
using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Authentication;

public class RefreshTests
{
    [Fact]
    public async Task Refresh_Ok()
    {
        // Arrange
        var accessToken = Guid.NewGuid().ToString();
        var refreshToken = Guid.NewGuid().ToString();
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<RefreshTokenRequest>(), default)).ReturnsAsync(new Token
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });

        var (requestData, context) = AzureFunctionContext.Generate(new RefreshTokenRequest(Guid.NewGuid().ToString()));

        // Act
        var controller = new Refresh(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = result.GetBody<Token>();
        token?.AccessToken.Should().Be(accessToken);
        token?.RefreshToken.Should().Be(refreshToken);
    }

    [Fact]
    public async Task Refresh_Unauthorized()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var (requestData, context) = AzureFunctionContext.Generate(new RefreshTokenRequest(Guid.NewGuid().ToString()));

        // Act
        var controller = new Refresh(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.Should().BeAssignableTo<UnauthorizedResult>();
    }
}