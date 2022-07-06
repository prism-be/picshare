// -----------------------------------------------------------------------
//  <copyright file = "GetPictureContentValidatorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using FluentAssertions;
using Prism.Picshare.Services.Photobooth.Live.Commands;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Live.Tests;

public class GetPictureContentValidatorTests
{

    [Fact]
    public void Validate_NoOrganisation()
    {
        // Arrange
        var request = new GetPictureContent(Guid.Empty, Guid.NewGuid());
        var validator = new GetPictureContentValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NoPicture()
    {
        // Arrange
        var request = new GetPictureContent(Guid.NewGuid(), Guid.Empty);
        var validator = new GetPictureContentValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new GetPictureContent(Guid.NewGuid(), Guid.NewGuid());
        var validator = new GetPictureContentValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}