// -----------------------------------------------------------------------
//  <copyright file="ValidationBehaviorTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Moq;
using Prism.Picshare.Events.Authentication;
using Prism.Picshare.Events.Behaviors;
using Prism.Picshare.Events.Exceptions;
using Xunit;

namespace Prism.Picshare.Events.Tests;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_Invalid()
    {
        // Arrange
        var validators = new List<IValidator<LoginRequest>> { new LoginRequestValidator() };

        var validationBehavior = new ValidationBehavior<LoginRequest, LoginResponse>(validators);

        // Act and Assert
        var loginRequest = new LoginRequest(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var exception = await Assert.ThrowsAsync<InvalidModelException>(async () => await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<LoginResponse>>()));

        // Assert
        Assert.NotNull(exception);
        Assert.NotEmpty(exception.Validations);
    }

    [Fact]
    public async Task Handle_NoValidators()
    {
        // Arrange
        var validationBehavior = new ValidationBehavior<LoginRequest, LoginResponse>(new List<IValidator<LoginRequest>>());

        // Act and Assert
        var loginRequest = new LoginRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<LoginResponse>>());
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var validators = new List<IValidator<LoginRequest>> { new LoginRequestValidator() };

        var validationBehavior = new ValidationBehavior<LoginRequest, LoginResponse>(validators);

        // Act
        var loginRequest = new LoginRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        var result = await validationBehavior.Handle(loginRequest, default, Mock.Of<RequestHandlerDelegate<LoginResponse>>());
        Assert.Null(result);
    }
}