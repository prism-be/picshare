// -----------------------------------------------------------------------
//  <copyright file = "EventsControllerTest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.AspNetCore.Controllers;

namespace Prism.Picshare.UnitTests;

public static class EventsControllerTest
{
    public static  async Task VerifyController<TController, TRequest, TExpectedCommand, TExpectedCommandReturn>()
        where TController : IEventExecutor<TRequest>
        where TExpectedCommand : IRequest<TExpectedCommandReturn>
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var controller = (IEventExecutor<TRequest>)Activator.CreateInstance(typeof(TController), mediator.Object)!;
        var result = await controller.Execute(Activator.CreateInstance<TRequest>());

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.VerifySend<TExpectedCommand, TExpectedCommandReturn>();
    }

    public static async Task VerifyController<TController, TRequest, TExpectedCommand>()
        where TController : IEventExecutor<TRequest>
        where TExpectedCommand : IRequest
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var controller = (IEventExecutor<TRequest>)Activator.CreateInstance(typeof(TController), mediator.Object)!;
        var result = await controller.Execute(Activator.CreateInstance<TRequest>());

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<TExpectedCommand>();
    }
}