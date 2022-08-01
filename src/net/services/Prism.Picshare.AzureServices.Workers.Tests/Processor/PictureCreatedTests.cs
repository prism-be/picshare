// -----------------------------------------------------------------------
//  <copyright file = "ExifReadTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Prism.Picshare.AzureServices.Workers.Pictures;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Processor;

public class PictureCreatedTests
{
    [Fact]
    public async Task Run_Null()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyNull<Created, EntityReference, ResultCodes, ReadMetaData>();
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyOk<Created, EntityReference, ResultCodes, ReadMetaData>(new EntityReference
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid()
        });
    }
}