// -----------------------------------------------------------------------
//  <copyright file = "AdminControllerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Moq;
using Prism.Picshare.Commands.Pictures.Admin;
using Prism.Picshare.Services.Api.Controllers;

namespace Prism.Picshare.Services.Api.Tests.Controllers;

public class AdminControllerTests
{
    [Fact]
    public async Task Create_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var userContextAccessor = UserContextMock.Generate();

        // Act
        await new AdminController(mediator.Object, userContextAccessor.Object).EventsCreated();

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchPictureEvents>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Updated_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var userContextAccessor = UserContextMock.Generate();

        // Act
        await new AdminController(mediator.Object, userContextAccessor.Object).EventsUpdated();

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchPictureEvents>(), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Uploaded_Ok()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var userContextAccessor = UserContextMock.Generate();

        // Act
        await new AdminController(mediator.Object, userContextAccessor.Object).EventsUploaded();

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<RelaunchUpload>(), It.IsAny<CancellationToken>()));
    }
}