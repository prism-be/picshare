// -----------------------------------------------------------------------
//  <copyright file = "UserRegisterTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Prism.Picshare.AzureServices.Workers.Mailing;
using Prism.Picshare.Commands.Mailing;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Mailing;

public class UserRegisterTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var user = new Domain.User();

        var mediator = new Mock<IMediator>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new UserRegister(mediator.Object);
        await controller.Run(JsonSerializer.Serialize(user), context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.Is<RegisterConfirmation>(r => r.RegisteringUser == user), It.IsAny<CancellationToken>()), Times.Once);
    }
}