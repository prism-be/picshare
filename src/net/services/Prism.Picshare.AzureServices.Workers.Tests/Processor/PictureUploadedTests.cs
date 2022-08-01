// -----------------------------------------------------------------------
//  <copyright file = "PictureUploadedTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Moq;
using Prism.Picshare.AzureServices.Workers.Pictures;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Processor;

public class PictureUploadedTests
{

    [Fact]
    public async Task Run_Null()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var publisherClient = new Mock<PublisherClient>();

        var context = new Mock<FunctionContext>();

        // Act
        var controller = new Uploaded(mediator.Object, publisherClient.Object);
        await controller.Run("null", context.Object);

        // Assert
        mediator.VerifySend<GenerateThumbnail, ResultCodes>(Times.Never());
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        var publisherClient = new Mock<PublisherClient>();

        var message = JsonSerializer.Serialize(new EntityReference
        {
            OrganisationId = organisationId,
            Id = pictureId
        });
        var context = new Mock<FunctionContext>();

        // Act
        var controller = new Uploaded(mediator.Object, publisherClient.Object);
        await controller.Run(message, context.Object);

        // Assert
        mediator.VerifySend<GenerateThumbnail, ResultCodes>(Times.Exactly(4));
        publisherClient.VerifyPublishEvent<EntityReference>(Topics.Pictures.ThumbnailsGenerated);
    }
}