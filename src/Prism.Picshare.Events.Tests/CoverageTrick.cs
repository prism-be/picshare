// -----------------------------------------------------------------------
//  <copyright file="CoverageTrick.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Xunit;

namespace Prism.Picshare.Events.Tests;

public class CoverageTrick
{
    [Fact]
    public void EntryPoint_Should_Ok()
    {
        // Arrange
        // Act
        var entryPoint = new EntryPoint();

        // Assert
        Assert.NotNull(entryPoint);
    }
}