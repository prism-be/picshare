// -----------------------------------------------------------------------
//  <copyright file = "MockMediatorExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Moq;

namespace Prism.Picshare.UnitTesting;

public static class MockMediatorExtensions
{
    public static void VerifySend<TExpected>(this Mock<IMediator> mock)
        where TExpected : IRequest
    {
        mock.Verify(x => x.Send(It.IsAny<TExpected>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    public static void VerifySend<TExpected, TResponse>(this Mock<IMediator> mock, Times times)
        where TExpected : IRequest<TResponse>
    {
        mock.Verify(x => x.Send(It.IsAny<TExpected>(), It.IsAny<CancellationToken>()), times);
    }

    public static void VerifySend<TExpected, TResponse>(this Mock<IMediator> mock)
        where TExpected : IRequest<TResponse>
    {
        mock.VerifySend<TExpected, TResponse>(Times.Once());
    }
}