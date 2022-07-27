﻿// -----------------------------------------------------------------------
//  <copyright file = "ThumbsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.AzureServices.Api.Pictures;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Pictures;

public class ThumbsTests
{

    [Fact]
    public async Task GetThumbs_Not_Found()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<Thumbs>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var blobClient = new Mock<BlobClient>();

        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Thumbs(logger.Object, blobClient.Object);
        var thumb = await controller.Run(requestData.Object, context.Object, Guid.NewGuid(), Guid.NewGuid(), 150, 150);

        // Assert
        thumb.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetThumbs_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<Thumbs>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var blobClient = new Mock<BlobClient>();
        blobClient.Setup(x => x.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Samples.SmallImage);

        var (requestData, context) = AzureFunctionContext.Generate(organisationId: organisationId);

        // Act
        var controller = new Thumbs(logger.Object, blobClient.Object);
        var thumb = await controller.Run(requestData.Object, context.Object, organisationId, Guid.NewGuid(), 150, 150);

        // Assert
        thumb.StatusCode.Should().Be(HttpStatusCode.OK);;
    }

    [Fact]
    public async Task GetThumbs_Unauthorized()
    {
        // Arrange
        var organisationId = Guid.NewGuid();

        var logger = new Mock<ILogger<Thumbs>>();

        var userContextAccessor = new Mock<IUserContextAccessor>();
        userContextAccessor.Setup(x => x.OrganisationId).Returns(organisationId);

        var blobClient = new Mock<BlobClient>();

        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        var controller = new Thumbs(logger.Object, blobClient.Object);
        var thumb = await controller.Run(requestData.Object, context.Object, Guid.NewGuid(), Guid.NewGuid(), 150, 150);

        // Assert
        thumb.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}