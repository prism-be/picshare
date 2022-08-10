// -----------------------------------------------------------------------
//  <copyright file = "PicturesTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Workers.Workers.Pictures;
using Prism.Picshare.UnitTesting;

namespace Prism.Picshare.Services.Workers.Tests.Workers;

public class PicturesTests
{

    [Fact]
    public async Task ExifRead_Ok()
    {
        await WorkersTesting.VerifyOk<PictureExifRead, Picture, Picture, GeneratePictureSummary>(
            Topics.Pictures.ExifRead,
            new Picture
            {
                OrganisationId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            });
    }

    [Fact]
    public async Task PictureCreated_Ok()
    {
        await WorkersTesting.VerifyOk<PictureCreated, Picture, ResultCodes, ReadMetaData>(
            Topics.Pictures.Created,
            new Picture
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                Owner = Guid.NewGuid()
            });
    }

    [Fact]
    public async Task PictureSeen_Ok()
    {
        await WorkersTesting.VerifyOk<PictureSeen, EntityReference, Picture, IncreaseViewCount>(
            Topics.Pictures.Seen,
            new EntityReference
            {
                OrganisationId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            });
    }

    [Fact]
    public async Task PictureSummaryUpdated_Ok()
    {
        await WorkersTesting.VerifyOk<PictureSummaryUpdated, PictureSummary, Flow, UpdateFlowSummary>(
            Topics.Pictures.SummaryUpdated,
            new PictureSummary
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid()
            });
    }

    [Fact]
    public async Task PictureThumbnailGenerated_Ok()
    {
        await WorkersTesting.VerifyOk<PictureThumbnailsGenerated, EntityReference, PictureSummary, SetPictureReady>(
            Topics.Pictures.ThumbnailsGenerated,
            new EntityReference
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid()
            });
    }

    [Fact]
    public async Task PictureUploaded_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        var publisherClient = new Mock<PublisherClient>();

        var message = new EntityReference
        {
            OrganisationId = organisationId,
            Id = pictureId
        };

        // Act
        var controller = new PictureUploaded(Mock.Of<ILogger<PictureUploaded>>(), Mock.Of<IServiceProvider>(), publisherClient.Object);
        await controller.ProcessMessageAsync(mediator.Object, message);

        // Assert
        mediator.VerifySend<GenerateThumbnail, ResultCodes>(Times.Exactly(4));
        publisherClient.VerifyPublishEvent<EntityReference>(Topics.Pictures.ThumbnailsGenerated);
    }
}