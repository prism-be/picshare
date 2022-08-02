// -----------------------------------------------------------------------
//  <copyright file = "PicturesControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Security;
using Prism.Picshare.Services.Api.Controllers;
using Prism.Picshare.UnitTests;

namespace Prism.Picshare.Services.Api.Tests.Controllers;

public class PicturesControllerTests
{
    [Fact]
    public async Task Flow_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId);
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, string.Empty, organisationId.ToString(), new Flow
        {
            Id = organisationId
        });
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, new Authorizations
        {
            Id = userId
        });
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var flow = await controller.Flow();

        // Assert
        flow.Should().BeAssignableTo<OkObjectResult>();
    }

    [Fact]
    public async Task Show_Forbidden()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId, false);
        var storeClient = new Mock<StoreClient>();
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var result = await controller.Show(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<NotFoundResult>();
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Never());
    }

    [Fact]
    public async Task Show_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId);
        var storeClient = new Mock<StoreClient>();
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();

        mediator.Setup(x => x.Send(It.IsAny<IncreaseViewCount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Picture
            {
                Id = organisationId,
                OrganisationId = Guid.NewGuid()
            });

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var result = await controller.Show(organisationId, Guid.NewGuid());

        // Assert
        result.Should().BeAssignableTo<OkObjectResult>();
        mediator.VerifySend<IncreaseViewCount, Picture>(Times.Once());
    }

    [Fact]
    public async Task Thumbs_Not_Found()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId);
        var storeClient = new Mock<StoreClient>();
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var thumb = await controller.Thumbs(Guid.NewGuid().ToString(), 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<NotFoundResult>();
    }

    [Fact]
    public async Task Thumbs_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId);
        var storeClient = new Mock<StoreClient>();
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();
        blobClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImage);

        var token = TokenGenerator.GeneratePictureToken(JwtConfigurationFake.JwtConfiguration.PrivateKey, organisationId, userId, Guid.NewGuid());

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var thumb = await controller.Thumbs(token, 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<FileResult>();
    }

    [Fact]
    public async Task Thumbs_Unauthorized()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userContextAccessor = UserContextMock.Generate(organisationId, userId, false);
        var storeClient = new Mock<StoreClient>();
        var mediator = new Mock<IMediator>();
        var logger = new Mock<ILogger<PicturesController>>();
        var blobClient = new Mock<BlobClient>();

        // Act
        var controller = new PicturesController(userContextAccessor.Object, storeClient.Object, mediator.Object, JwtConfigurationFake.JwtConfiguration, logger.Object, blobClient.Object);
        var thumb = await controller.Thumbs(string.Empty, 150, 150);

        // Assert
        thumb.Should().BeAssignableTo<NotFoundResult>();
    }
}