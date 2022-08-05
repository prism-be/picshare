// -----------------------------------------------------------------------
//  <copyright file = "MailingTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Commands.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Workers.Workers.Email;

namespace Prism.Picshare.Services.Workers.Tests.Workers;

public class MailingTests
{
    [Fact]
    public async Task EmailValidated_Ok()
    {
        await WorkersTesting.VerifyOk<EmailValidated, User, ResultCodes, EmailValidatedRequest>(Topics.Email.Validated, new User
        {
            Id = Guid.NewGuid()
        });
    }
}