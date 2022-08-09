// -----------------------------------------------------------------------
//  <copyright file = "WorkersTesting.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Services.Workers.Workers;

namespace Prism.Picshare.Services.Workers.Tests;

public static class WorkersTesting
{
    public static async Task VerifyOk<TWorker, TData, TExpectedRequest>(string queue, TData data)
        where TWorker : BaseServiceBusWorker<TData>
        where TExpectedRequest : IRequest
    {
        var logger = new Mock<ILogger<TWorker>>();
        var mediator = new Mock<IMediator>();

        // Act
        var worker = (TWorker)Activator.CreateInstance(typeof(TWorker), logger.Object, Mock.Of<IServiceProvider>())!;
        await worker.ProcessMessageAsync(mediator.Object, data);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        worker.Queue.Should().Be(queue);
    }

    public static async Task VerifyOk<TWorker, TData, TReturn, TExpectedRequest>(string queue, TData data)
        where TWorker : BaseServiceBusWorker<TData>
        where TExpectedRequest : IRequest<TReturn>
    {
        var logger = new Mock<ILogger<TWorker>>();
        var mediator = new Mock<IMediator>();

        // Act
        var worker = (TWorker)Activator.CreateInstance(typeof(TWorker), logger.Object, Mock.Of<IServiceProvider>())!;
        await worker.ProcessMessageAsync(mediator.Object, data);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<TExpectedRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        worker.Queue.Should().Be(queue);
    }
}