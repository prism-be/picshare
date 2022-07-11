// -----------------------------------------------------------------------
//  <copyright file = "PhotoboothPictureUploadedTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Albums;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Events;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Events;

public class PhotoboothPictureUploadedTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<PhotoboothPictureUploaded>>();
        var mediator = new Mock<IMediator>();

        var photoboothPicture = new PhotoboothPicture
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            OriginalFileName = "Hellow World.jpg"
        };

        // Act
        var controller = new PhotoboothPictureUploaded(logger.Object, mediator.Object);
        var result = await controller.Handle(photoboothPicture);

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.Verify(x => x.Send(new InitializePicture(photoboothPicture.OrganisationId, photoboothPicture.OrganisationId, photoboothPicture.Id, PictureSource.Photobooth), default), Times.Once);
        mediator.Verify(x => x.Send(new SetPictureName(photoboothPicture.OrganisationId, photoboothPicture.Id, photoboothPicture.OriginalFileName), default), Times.Once);
        mediator.Verify(x => x.Send(new AddPictureToAlbum(photoboothPicture.OrganisationId, photoboothPicture.SessionId, photoboothPicture.Id), default), Times.Once);
    }
}