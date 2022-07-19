// -----------------------------------------------------------------------
//  <copyright file = "UpdateFlowSummaryTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Pictures;

public class UpdateFlowSummaryTests
{

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrangeµ
        var organisationId = Guid.NewGuid();
        var summary = new PictureSummary
        {
            OrganisationId = organisationId,
            Date = DateTime.UtcNow,
            Id = Guid.NewGuid()
        };

        var flow = new Flow
        {
            OrganisationId = organisationId,
            Pictures = new List<PictureSummary>
            {
                new()
                {
                    Date = DateTime.UtcNow.AddDays(-1),
                    Id = Guid.NewGuid()
                },
                summary
            }
        };

        var logger = new Mock<ILogger<UpdateFlowSummaryHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, flow.OrganisationId.ToString(), flow);

        // Act
        var handler = new UpdateFlowSummaryHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(new UpdateFlowSummary(summary), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Pictures.Count.Should().Be(2);
        result.Pictures.First().Should().Be(summary);
        storeClient.VerifySaveState<Flow>(Stores.Flow);
    }

    [Fact]
    public async Task Handle_Ok_No_Flow()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var summary = new PictureSummary
        {
            OrganisationId = organisationId,
            Date = DateTime.UtcNow,
            Id = Guid.NewGuid()
        };

        var logger = new Mock<ILogger<UpdateFlowSummaryHandler>>();
        var storeClient = new Mock<StoreClient>();

        // Act
        var handler = new UpdateFlowSummaryHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(new UpdateFlowSummary(summary), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Pictures.Count.Should().Be(1);
        result.Pictures.First().Should().Be(summary);
        storeClient.VerifySaveState<Flow>(Stores.Flow, Times.Once());
    }

    [Fact]
    public async Task Handle_Ok_Not_Summary()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var summary = new PictureSummary
        {
            OrganisationId = organisationId,
            Date = DateTime.UtcNow,
            Id = Guid.NewGuid()
        };

        var flow = new Flow
        {
            OrganisationId = organisationId,
            Pictures = new List<PictureSummary>
            {
                new()
                {
                    Date = DateTime.UtcNow.AddDays(-1),
                    Id = Guid.NewGuid()
                }
            }
        };

        var logger = new Mock<ILogger<UpdateFlowSummaryHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, flow.OrganisationId.ToString(), flow);

        // Act
        var handler = new UpdateFlowSummaryHandler(logger.Object, storeClient.Object);
        var result = await handler.Handle(new UpdateFlowSummary(summary), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Pictures.Count.Should().Be(2);
        result.Pictures.First().Should().Be(summary);
        storeClient.VerifySaveState<Flow>(Stores.Flow);
    }
}