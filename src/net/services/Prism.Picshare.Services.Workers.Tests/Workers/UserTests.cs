// -----------------------------------------------------------------------
//  <copyright file = "UserTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Workers.Workers.User;

namespace Prism.Picshare.Services.Workers.Tests.Workers;

public class UserTests
{
    [Fact]
    public async Task UserRegister_Ok()
    {
        await WorkersTesting.VerifyOk<UserRegister, User, RegisterConfirmation>(
            Topics.User.Register,
            new User
            {
                Id = Guid.NewGuid()
            });
    }
}