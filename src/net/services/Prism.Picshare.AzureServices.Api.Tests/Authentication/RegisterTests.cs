// -----------------------------------------------------------------------
//  <copyright file = "RegisterTests.cs" company = "Prism">
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
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Authentication;

public class RegisterTests
{
    [Theory]
    [InlineData(ResultCodes.Ok, HttpStatusCode.NoContent)]
    [InlineData(ResultCodes.ExistingOrganisation, HttpStatusCode.Conflict)]
    [InlineData(ResultCodes.ExistingUsername, HttpStatusCode.Conflict)]
    public async Task Register(ResultCodes code, HttpStatusCode statusCode)
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<RegisterAccountRequest>(), default)).ReturnsAsync(code);

        var (requestData, context) = AzureFunctionContext.Generate(new RegisterAccountRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()));

        // Act
        var controller = new Register(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.StatusCode.Should().Be(statusCode);
    }
}