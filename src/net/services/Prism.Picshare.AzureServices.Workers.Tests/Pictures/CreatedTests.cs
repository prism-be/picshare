// -----------------------------------------------------------------------
//  <copyright file = "ExifReadTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Prism.Picshare.AzureServices.Workers.Pictures;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Pictures;

public class CreatedTests
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
        await WorkersTesting.VerifyOk<Created, EntityReference, ResultCodes, ReadMetaData>(new Picture
        {
            Id = Guid.NewGuid(),
            OrganisationId = Guid.NewGuid(),
            Owner = Guid.NewGuid()
        });
    }
}