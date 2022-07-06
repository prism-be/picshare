// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Acme.Dapr.Extensions.UnitTesting;
using Dapr.Client;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Model;
using Prism.Picshare.Services.Mailing.Workers;
using Xunit;

namespace Prism.Picshare.Services.Mailing.Tests.Commands;

public class RegisterConfirmationTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new RegisterConfirmation(new User());
        var daprClient = new Mock<DaprClient>();
        var emailWorker = new Mock<IEmailWorker>();

        // Act
        var handler = new RegisterConfirmationHandler(daprClient.Object, emailWorker.Object, Mock.Of<MailingConfiguration>());
        await handler.Handle(request, CancellationToken.None);

        // Assert
        daprClient.VerifySaveState<MailAction<User>>(Stores.MailActions);
        emailWorker.Verify(x => x.RenderAndSendAsync("register-confirmation", request.RegisteringUser, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}