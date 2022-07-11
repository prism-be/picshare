// -----------------------------------------------------------------------
//  <copyright file = "RelaunchCreatedTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using FluentAssertions;
using MediatR;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Admin;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Commands.Admin;

public class RelaunchPictureEventsTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var daprClient = new Mock<DaprClient>();
        daprClient.Setup(x => x.GetStateAsync<Flow>(Stores.Flow, organisationId.ToString(), null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Flow
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
        var handler = new RelaunchPictureEventsHandler(daprClient.Object);
        var result = await handler.Handle(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        daprClient.Verify(x => x.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Updated, It.IsAny<Picture>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
    
    [Fact]
    public async Task Handle_No_Flow()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new RelaunchPictureEventsHandler(daprClient.Object);
        var result = await handler.Handle(new RelaunchPictureEvents(organisationId, Topics.Pictures.Updated), CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
        daprClient.Verify(x => x.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Updated, It.IsAny<Picture>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}