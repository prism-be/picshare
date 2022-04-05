// -----------------------------------------------------------------------
//  <copyright file="PictureTakenHandlerTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using LiteDB;
using Moq;
using Prism.Picshare.Data;
using Prism.Picshare.Events.Photobooth;
using Prism.Picshare.Services.Photobooth.Data;
using Prism.Picshare.Services.Photobooth.Handlers;
using Xunit;

namespace Prism.Picshare.Services.Photobooth.Tests;

public class PictureTakenHandlerTests
{
    [Fact]
    public void Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid().ToString();
        var sessionId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();

        using var liteDbMemory = new MemoryStream();
        using var liteDb = new LiteDatabase(liteDbMemory);

        var databaseResolver = new Mock<IDatabaseResolver>();
        databaseResolver.Setup(x => x.GetDatabase(organisationId, "photobooth")).Returns(liteDb);

        var handler = new PictureTakenHandler(databaseResolver.Object);

        // Act
        var request = new PictureTaken(organisationId, sessionId, pictureId, DateTime.Now);
        handler.Handle(request, default);

        // Assert
        var dbPicture = liteDb.GetCollection<Pictures>().FindById(pictureId);
        Assert.NotNull(dbPicture);
        Assert.Equal(sessionId, dbPicture.Session);
    }
}