// -----------------------------------------------------------------------
//  <copyright file = "ValidationBehaviorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Exceptions;
using Xunit;

namespace Prism.Picshare.Tests;

public class LogCommandsBehaviorTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<LogCommandsBehavior<DummyRequest, DummyResponse>>>();

        var logCommandsBehavior = new LogCommandsBehavior<DummyRequest, DummyResponse>(logger.Object);

        // Act and Assert
        var loginRequest = new DummyRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        await logCommandsBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<DummyResponse>>());

        // Assert
        logger.Invocations.Should().NotBeEmpty();
    }
}