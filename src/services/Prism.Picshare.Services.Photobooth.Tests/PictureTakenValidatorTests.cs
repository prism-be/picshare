// -----------------------------------------------------------------------
//  <copyright file="PictureTakenValidatorTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Prism.Picshare.Services.Photobooth.Commands;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Tests;

public class PictureTakenValidatorTests
{
    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var validator = new PictureTakenValidator();
        var pictureTaken = new PictureTaken(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = validator.Validate(pictureTaken);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Empty_Organisation()
    {
        // Arrange
        var validator = new PictureTakenValidator();
        var pictureTaken = new PictureTaken(Guid.Empty, Guid.NewGuid());

        // Act
        var result = validator.Validate(pictureTaken);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_Empty_Session()
    {
        // Arrange
        var validator = new PictureTakenValidator();
        var pictureTaken = new PictureTaken(Guid.NewGuid(), Guid.Empty);

        // Act
        var result = validator.Validate(pictureTaken);

        // Assert
        Assert.False(result.IsValid);
    }
}