// -----------------------------------------------------------------------
//  <copyright file = "UploadControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;
using Prism.Picshare.Services.Pictures.Controllers.Api;
using Xunit;

namespace Prism.Picshare.Services.Pictures.Tests.Controllers.Api;

public class UploadControllerTests
{
    [Fact]
    public async Task Upload_Ok()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var stream = new MemoryStream(data);
        
        var logger = new Mock<ILogger<UploadController>>();
        var mediator = new Mock<IMediator>();
        var userContextAccessor = new Mock<IUserContextAccessor>();

        // Act
        var controller = new UploadController(logger.Object, mediator.Object, userContextAccessor.Object);
        var result = await controller.Upload(new FormFile(stream, 0, 42, "unit-test.jpg", "unit-test.jpg"));

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<UploadPicture>();
        mediator.VerifySend<InitializePicture, Picture>();
        mediator.VerifySend<SetPictureName, Picture>();
    }
}