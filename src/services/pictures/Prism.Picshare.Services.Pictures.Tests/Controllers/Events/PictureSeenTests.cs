// -----------------------------------------------------------------------
//  <copyright file = "PictureSeenTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Events;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Events;

public class PictureSeenTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<PictureSeen>>();
        var mediator = new Mock<IMediator>();

        var picture = new EntityReference
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        };

        // Act
        var controller = new PictureSeen(logger.Object, mediator.Object);
        var result = await controller.Handle(picture);

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.Verify(x => x.Send(new IncreaseViewCount(picture.OrganisationId, picture.Id), default), Times.Once);
    }
}