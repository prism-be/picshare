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
using Prism.Picshare.AzureServices.Api.Pictures;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Pictures;

public class UploadControllerTests
{
    [Fact]
    public async Task Upload_Ok()
    {
        // Arrange
        var data = new byte[42];
        Random.Shared.NextBytes(data);
        var stream = new MemoryStream(data);

        var logger = new Mock<ILogger<Upload>>();
        var mediator = new Mock<IMediator>();

        var (requestData, context) = AzureFunctionContext.Generate(new FormFile(stream, 0, 42, "unit-test.jpg", "unit-test.jpg"));

        // Act
        var controller = new Upload(logger.Object, mediator.Object);
        var result = await controller.Run(requestData.Object, context.Object);

        // Assert
        result.Should().BeAssignableTo<OkResult>();
        mediator.VerifySend<UploadPicture>();
        mediator.VerifySend<InitializePicture, Picture>();
        mediator.VerifySend<SetPictureName, Picture>();
    }
}