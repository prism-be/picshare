// -----------------------------------------------------------------------
//  <copyright file = "RelaunchPictureEventsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Admin;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Admin;

public class RelaunchPictureEventsTests
{

    [Fact]
    public async Task Handle_No_Flow()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var publisherClient = new Mock<IPublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, organisationId.ToString(), new Flow());

        // Act
        var handler = new RelaunchPictureEventsHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        publisherClient.VerifyPublishEvent<Picture>(Topics.Pictures.Updated, Times.Never());
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var publisherClient = new Mock<IPublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, organisationId.ToString(), new Flow
        {
            OrganisationId = organisationId,
            Pictures = new List<PictureSummary>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisationId
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisationId
                }
            }
        });

        // Act
        var handler = new RelaunchPictureEventsHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        publisherClient.VerifyPublishEvent<Picture>(Topics.Pictures.Updated, Times.Exactly(2));
    }
}