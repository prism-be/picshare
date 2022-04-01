// -----------------------------------------------------------------------
//  <copyright file="LoginRequestTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Events.Authentication;
using Xunit;

namespace Prism.Picshare.Events.Tests.Authentication;

public class LoginRequestTests
{
    [Fact]
    public void LoginRequest_Validation_NotNull_Login()
    {
        // Arrange
        var validator = new LoginRequestValidator();

        // Act
        var result = validator.Validate(new LoginRequest("test", null!, "test"));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void LoginRequest_Validation_NotNull_Organisation()
    {
        // Arrange
        var validator = new LoginRequestValidator();

        // Act
        var result = validator.Validate(new LoginRequest(null!, "test", "test"));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void LoginRequest_Validation_NotNull_Password()
    {
        // Arrange
        var validator = new LoginRequestValidator();

        // Act
        var result = validator.Validate(new LoginRequest("test", "test", null!));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void LoginRequest_Validation_Valid()
    {
        // Arrange
        var validator = new LoginRequestValidator();

        // Act
        var result = validator.Validate(new LoginRequest("test", "test", "test"));

        // Assert
        Assert.True(result.IsValid);
    }
}