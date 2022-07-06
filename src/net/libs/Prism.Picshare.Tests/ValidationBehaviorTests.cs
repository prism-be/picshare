// -----------------------------------------------------------------------
//  <copyright file = "ValidationBehaviorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Moq;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Exceptions;
using Xunit;

namespace Prism.Picshare.Tests;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_Invalid()
    {
        // Arrange
        var validators = new List<IValidator<DummyRequest>>
        {
            new DummyRequestValidator()
        };

        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(validators);

        // Act and Assert
        var loginRequest = new DummyRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var exception = await Assert.ThrowsAsync<InvalidModelException>(async () => await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<DummyResponse>>()));

        // Assert
        Assert.NotNull(exception);
        Assert.NotEmpty(exception.Validations);
    }

    [Fact]
    public async Task Handle_NoValidators()
    {
        // Arrange
        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(new List<IValidator<DummyRequest>>());

        // Act and Assert
        var loginRequest = new DummyRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<DummyResponse>>());
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var validators = new List<IValidator<DummyRequest>>
        {
            new DummyRequestValidator()
        };

        var validationBehavior = new ValidationBehavior<DummyRequest, DummyResponse>(validators);

        // Act
        var loginRequest = new DummyRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<DummyResponse>>());
        Assert.Null(result);
    }
}