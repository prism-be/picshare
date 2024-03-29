﻿// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Prism.Picshare.Commands.Mailing;
using Prism.Picshare.Domain;
using Prism.Picshare.Mailing;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Commands.Tests.Mailing;

public class RegisterConfirmationTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new RegisterConfirmation(new User());
        var client = new Mock<StoreClient>();
        var emailWorker = new Mock<IEmailWorker>();

        // Act
        var handler = new RegisterConfirmationHandler(client.Object, emailWorker.Object, Mock.Of<MailingConfiguration>());
        await handler.Handle(request, CancellationToken.None);

        // Assert
        client.VerifySaveState<MailAction<User>>(Stores.MailActions);
        emailWorker.Verify(x => x.RenderAndSendAsync("register-confirmation", request.RegisteringUser, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}