// -----------------------------------------------------------------------
//  <copyright file = "WorkersTesting.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Moq;

namespace Prism.Picshare.AzureServices.Workers.Tests;

public static class WorkersTesting
{

    public static async Task VerifyNull<TWorker, TExpectedRequest>() where TWorker : ISimpleFunction where TExpectedRequest : IRequest
    {
        var mediator = new Mock<IMediator>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = (TWorker)Activator.CreateInstance(typeof(TWorker), mediator.Object)!;
        await controller.Run("null", context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>()!, It.IsAny<CancellationToken>()), Times.Never);
    }

    // ReSharper disable once UnusedTypeParameter
    public static async Task VerifyNull<TWorker, TData, TReturn, TExpectedRequest>() where TWorker : ISimpleFunction where TExpectedRequest : IRequest<TReturn>
    {
        var mediator = new Mock<IMediator>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = (TWorker)Activator.CreateInstance(typeof(TWorker), mediator.Object)!;
        await controller.Run("null", context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>()!, It.IsAny<CancellationToken>()), Times.Never);
    }

    public static async Task VerifyOk<TWorker, TData, TExpectedRequest>(TData data) where TWorker : ISimpleFunction where TExpectedRequest : IRequest
    {
        var mediator = new Mock<IMediator>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = (TWorker)Activator.CreateInstance(typeof(TWorker), mediator.Object)!;
        await controller.Run(JsonSerializer.Serialize(data), context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public static async Task VerifyOk<TWorker, TData, TReturn, TExpectedRequest>(TData data) where TWorker : ISimpleFunction where TExpectedRequest : IRequest<TReturn>
    {
        var mediator = new Mock<IMediator>();
        var context = new Mock<FunctionContext>();

        // Act
        var controller = (TWorker)Activator.CreateInstance(typeof(TWorker), mediator.Object)!;
        await controller.Run(JsonSerializer.Serialize(data), context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}