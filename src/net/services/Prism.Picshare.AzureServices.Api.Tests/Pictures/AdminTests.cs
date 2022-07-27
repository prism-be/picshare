// -----------------------------------------------------------------------
//  <copyright file = "AdminTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Prism.Picshare.AzureServices.Api.Admin.Events;
using Prism.Picshare.Commands.Pictures.Admin;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Api.Tests.Pictures;

public class AdminTests
{
    [Fact]
    public async Task Create_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        await new Created(mediator.Object).Run(requestData.Object, context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchPictureEvents>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Updated_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        await new Updated(mediator.Object).Run(requestData.Object, context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchPictureEvents>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Uploaded_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var (requestData, context) = AzureFunctionContext.Generate();

        // Act
        await new Uploaded(mediator.Object).Run(requestData.Object, context.Object);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchUpload>(), It.IsAny<CancellationToken>()));
    }
}