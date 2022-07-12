// -----------------------------------------------------------------------
//  <copyright file = "ManagementControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Services.Pictures.Commands.Admin;
using Prism.Picshare.Services.Pictures.Controllers.Events;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Events;

public class ManagementControllerTests
{

    [Fact]
    public async Task RelaunchCreated_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var result = await new ManagementController(mediator.Object).RelaunchCreated(Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<RelaunchPictureEvents>();
    }

    [Fact]
    public async Task RelaunchUpdated_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var result = await new ManagementController(mediator.Object).RelaunchUpdated(Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<RelaunchPictureEvents>();
    }

    [Fact]
    public async Task RelaunchUpload_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var result = await new ManagementController(mediator.Object).RelaunchUpload(Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<RelaunchUpload>();
    }
}