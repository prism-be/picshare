// -----------------------------------------------------------------------
//  <copyright file = "ShowTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.AzureServices.Api.Pictures;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Pictures;

public class ShowTests
{
    [Fact]
    public async Task GetPicture_Forbidden()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();

        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Show(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object, organisationId, Guid.NewGuid());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Never());
    }

    [Fact]
    public async Task GetPicture_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.HasAccess(organisationId)).Returns(true);
        var mediator = new Mock<IMediator>();
        mediator.Setup(x => x.Send(It.IsAny<IncreaseViewCount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Picture
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid()
            });

        var (requestData, context) = AzureFunctionContext.Generate(organisationId: organisationId);

        // Act
        var controller = new Show(mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object, organisationId, Guid.NewGuid());

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.GetBody<Picture>().Should().NotBeNull();
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Once());
    }
}