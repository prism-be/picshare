// -----------------------------------------------------------------------
//  <copyright file = "ExifReadTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Prism.Picshare.AzureServices.Workers.Pictures;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Pictures;

public class ThumbnailGeneratedTests
{
    [Fact]
    public async Task Run_Null()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyNull<ThumbnailsGenerated, EntityReference, PictureSummary, SetPictureReady>();
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyOk<ThumbnailsGenerated, EntityReference, PictureSummary, SetPictureReady>(new EntityReference
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        });
    }
}