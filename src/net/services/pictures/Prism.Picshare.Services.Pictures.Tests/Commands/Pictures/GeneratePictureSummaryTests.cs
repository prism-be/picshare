// -----------------------------------------------------------------------
//  <copyright file = "GeneratePictureSummaryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class GeneratePictureSummaryTests
{
    [Fact]
    public async Task Handle_Ok_All()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(daprClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(Guid.NewGuid(), Guid.NewGuid(), new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeOriginal",
                Value = JsonSerializer.SerializeToElement("2019:02:02 08:51:07")
            },
            new()
            {
                Tag = "DateTimeDigitized",
                Value = JsonSerializer.SerializeToElement("2019:02:02 08:53:07")
            },
            new()
            {
                Tag = "DateTime",
                Value = JsonSerializer.SerializeToElement("2019:02:10 14:37:10")
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 2, 8, 51, 7, DateTimeKind.Utc));
    }

    [Fact]
    public async Task Handle_Ok_All_But_Invalid()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(daprClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(Guid.NewGuid(), Guid.NewGuid(), new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeOriginal",
                Value = JsonSerializer.SerializeToElement("2019:02:oups 08:51:07")
            },
            new()
            {
                Tag = "DateTimeDigitized",
                Value = JsonSerializer.SerializeToElement("2019:02:02blibla08:53:07")
            },
            new()
            {
                Tag = "DateTime",
                Value = JsonSerializer.SerializeToElement("2019:02:10 14:37:10")
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 10, 14, 37, 10, DateTimeKind.Utc));
        daprClient.VerifyPublishEvent<PictureSummary>(Publishers.PubSub, Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_Digitized()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(daprClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(Guid.NewGuid(), Guid.NewGuid(), new List<ExifData>
        {
            new()
            {
                Tag = "DateTimeDigitized",
                Value = JsonSerializer.SerializeToElement("2019:02:02 08:53:07")
            },
            new()
            {
                Tag = "DateTime",
                Value = JsonSerializer.SerializeToElement("2019:02:10 14:37:10")
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 2, 8, 53, 7, DateTimeKind.Utc));
        daprClient.VerifyPublishEvent<PictureSummary>(Publishers.PubSub, Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_None()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(daprClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(Guid.NewGuid(), Guid.NewGuid(), new List<ExifData>()), CancellationToken.None);

        // Assert
        picture.CreationDate.Should().BeAfter(DateTime.UtcNow.AddMinutes(-1));
        daprClient.VerifyPublishEvent<PictureSummary>(Publishers.PubSub, Topics.Pictures.SummaryUpdated);
    }

    [Fact]
    public async Task Handle_Ok_OnlyDate()
    {
        // Arrange
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Pictures, It.IsAny<string>(), new Picture());

        // Act
        var handler = new GeneratePictureSummaryHandler(daprClient.Object);
        var picture = await handler.Handle(new GeneratePictureSummary(Guid.NewGuid(), Guid.NewGuid(), new List<ExifData>
        {
            new()
            {
                Tag = "DateTime",
                Value = JsonSerializer.SerializeToElement("2019:02:10 14:37:10")
            }
        }), CancellationToken.None);

        // Assert
        picture.Summary.Date.Should().Be(new DateTime(2019, 2, 10, 14, 37, 10, DateTimeKind.Utc));
        daprClient.VerifyPublishEvent<PictureSummary>(Publishers.PubSub, Topics.Pictures.SummaryUpdated);
    }
}