// -----------------------------------------------------------------------
//  <copyright file = "SummaryUpdatedControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Events;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Events;

public class SummaryUpdatedControllerTests
{
    [Fact]
    public async Task Execute_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        // Act
        var controller = new SummaryUpdatedController(mediator.Object);
        var result = await controller.Execute(new PictureSummary());

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.VerifySend<UpdateFlowSummary, Flow>();
    }
}