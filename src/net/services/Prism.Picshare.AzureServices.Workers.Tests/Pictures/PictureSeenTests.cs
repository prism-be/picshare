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
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Pictures;

public class PictureSeenTests
{
    [Fact]
    public async Task Run_Null()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyNull<PictureSeen, EntityReference, Picture, IncreaseViewCount>();
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyOk<PictureSeen, EntityReference, Picture, IncreaseViewCount>(new EntityReference
        {
            OrganisationId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        });
    }
}