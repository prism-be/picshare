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

public class ExifReadTests
{
    [Fact]
    public async Task Run_Null()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyNull<ExifRead, Picture, Picture, GeneratePictureSummary>();
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyOk<ExifRead, Picture, Picture, GeneratePictureSummary>(new Picture
        {
            OrganisationId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        });
    }
}