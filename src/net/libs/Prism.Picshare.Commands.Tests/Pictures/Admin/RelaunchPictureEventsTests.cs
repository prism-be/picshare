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
using Prism.Picshare.Commands.Pictures.Admin;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Pictures.Admin;

public class RelaunchPictureEventsTests
{

    [Fact]
    public async Task Handle_No_Flow()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Flow, string.Empty, organisationId.ToString(), new Flow());

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
        var publisherClient = new Mock<PublisherClient>();
        var storeClient = new Mock<StoreClient>();

        var existingPictureId = Guid.NewGuid();
        storeClient.SetupGetStateAsync(Stores.Flow, string.Empty, organisationId.ToString(), new Flow
        {
            Id = organisationId,
            Pictures = new List<PictureSummary>
            {
                new()
                {
                    Id = existingPictureId,
                    OrganisationId = organisationId
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisationId
                }
            }
        });
        
        storeClient.SetupGetStateAsync(Stores.Pictures, organisationId, existingPictureId, new Picture());

        // Act
        var handler = new RelaunchPictureEventsHandler(storeClient.Object, publisherClient.Object);
        var result = await handler.Handle(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        publisherClient.VerifyPublishEvents<Picture>(Topics.Pictures.Updated, Times.Exactly(1));
    }
}