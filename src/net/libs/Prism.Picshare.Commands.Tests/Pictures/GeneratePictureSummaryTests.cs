// -----------------------------------------------------------------------
//  <copyright file = "GeneratePictureSummaryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures;

public class GeneratePictureSummaryTests
{
    [Fact]
    public async Task Handle_Ok_All()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(organisationId, pictureId, new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeOriginal",
                Value = "2019:02:02 08:51:07"
            },
            new()
            {
                Tag = "DateTimeDigitized",
                Value = "2019:02:02 08:53:07"
            },
            new()
            {
                Tag = "DateTime",
                Value = "2019:02:10 14:37:10"
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 2, 8, 51, 7, DateTimeKind.Utc));
    }

    [Fact]
    public async Task Handle_Ok_All_But_Invalid()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(organisationId, pictureId, new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeOriginal",
                Value = "2019:02:oups 08:51:07"
            },
            new()
            {
                Tag = "DateTimeDigitized",
                Value = "2019:02:02blibla08:53:07"
            },
            new()
            {
                Tag = "DateTime",
                Value = "2019:02:10 14:37:10"
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 10, 14, 37, 10, DateTimeKind.Utc));
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_Digitized()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(organisationId, pictureId, new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeDigitized",
                Value = "2019:02:02 08:53:07"
            },
            new()
            {
                Tag = "DateTime",
                Value = "2019:02:10 14:37:10"
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 2, 8, 53, 7, DateTimeKind.Utc));
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_None()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(organisationId, pictureId, new List<ExifData>()), CancellationToken.None);

        // Assert
        picture.CreationDate.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_OnlyDate()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, pictureId, new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(storeClient.Object, publisherClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(organisationId, pictureId, new List<ExifData>
        {
            new()
            {
                Tag = "DateTime",
                Value = "2019:02:10 14:37:10"
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 10, 14, 37, 10, DateTimeKind.Utc));
        publisherClient.VerifyPublishEvent<PictureSummary>(Topics.Pictures.SummaryUpdated);
    }
}