// -----------------------------------------------------------------------
//  <copyright file = "UserRegisterTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Prism.Picshare.AzureServices.Workers.User;
using Prism.Picshare.Commands.Mailing;
using Xunit;

namespace Prism.Picshare.AzureServices.Workers.Tests.Mailing;

public class UserRegisterTests
{
    [Fact]
    public async Task Run_Null()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyNull<Register, RegisterConfirmation>();
    }

    [Fact]
    public async Task Run_Ok()
    {
        // Arrange, Act and Assert
        await WorkersTesting.VerifyOk<Register, Domain.User, RegisterConfirmation>(new Domain.User
        {
            Id = Guid.NewGuid()
        });
    }
}