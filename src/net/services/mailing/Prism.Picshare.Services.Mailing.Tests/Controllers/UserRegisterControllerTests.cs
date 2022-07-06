// -----------------------------------------------------------------------
//  <copyright file = "UserRegisterControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Controllers.Events;
using Xunit;

namespace Prism.Picshare.Services.Mailing.Tests.Controllers;

public class UserRegisterControllerTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var user = new User();
        
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<UserRegisterController>>();

        // Act
        var controller = new UserRegisterController(mediator.Object, logger.Object);
        var result = await controller.Handle(user);

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmation>(r => r.RegisteringUser == user), It.IsAny<CancellationToken>()), Times.Once);
    }
}