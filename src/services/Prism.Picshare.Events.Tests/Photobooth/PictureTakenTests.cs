// -----------------------------------------------------------------------
//  <copyright file="PictureTakenTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Prism.Picshare.Events.Photobooth;
using Xunit;

namespace Prism.Picshare.Events.Tests.Photobooth;

public class PictureTakenTests
{
    [Fact]
    public void PictureTakenValidator_MustHave_DateTaken()
    {
        // Arrange
        var validator = new PictureTakenValidator();

        // Act
        var result = validator.Validate(new PictureTaken("test", Guid.NewGuid(), Guid.NewGuid(), DateTime.MinValue));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void PictureTakenValidator_MustHave_Id()
    {
        // Arrange
        var validator = new PictureTakenValidator();

        // Act
        var result = validator.Validate(new PictureTaken("test", Guid.NewGuid(), Guid.Empty, DateTime.Now));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void PictureTakenValidator_MustHave_Organisation()
    {
        // Arrange
        var validator = new PictureTakenValidator();

        // Act
        var result = validator.Validate(new PictureTaken("", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void PictureTakenValidator_MustHave_Session()
    {
        // Arrange
        var validator = new PictureTakenValidator();

        // Act
        var result = validator.Validate(new PictureTaken("test", Guid.Empty, Guid.NewGuid(), DateTime.Now));

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void PictureTakenValidator_Ok()
    {
        // Arrange
        var validator = new PictureTakenValidator();

        // Act
        var result = validator.Validate(new PictureTaken("test", Guid.NewGuid(), Guid.NewGuid(), DateTime.Now));

        // Assert
        Assert.True(result.IsValid);
    }
}