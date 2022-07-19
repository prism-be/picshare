// -----------------------------------------------------------------------
//  <copyright file = "PictureUploadedControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Processor.Commands;
using Prism.Picshare.Services.Processor.Controllers.Events;
using Prism.Picshare.UnitTests;

namespace Prism.Picshare.Services.Processor.Tests.Controllers.Events;

public class PictureUploadedControllerTests
{
    [Fact]
    public async Task PictureUploaded_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var mediator = new Mock<IMediator>();
        var publisherClient = new Mock<PublisherClient>();

        // Act
        var controller = new PictureUploadedController(mediator.Object, publisherClient.Object);
        var result = await controller.PictureUploaded(new EntityReference
        {
            OrganisationId = organisationId,
            Id = pictureId
        });

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<GenerateThumbnail, ResultCodes>(Times.Exactly(4));
        publisherClient.VerifyPublishEvent<EntityReference>(Topics.Pictures.ThumbnailsGenerated);
    }
}