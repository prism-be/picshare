// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Mailing.Commands;
using Prism.Picshare.Services.Mailing.Workers;
using Prism.Picshare.UnitTests;
using Xunit;

namespace Prism.Picshare.Services.Mailing.Tests.Commands;

public class RegisterConfirmationTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var request = new RegisterConfirmation(new User());
        var client = new Mock<IStoreClient>();
        var emailWorker = new Mock<IEmailWorker>();

        // Act
        var handler = new RegisterConfirmationHandler(client.Object, emailWorker.Object, Mock.Of<MailingConfiguration>());
        await handler.Handle(request, CancellationToken.None);

        // Assert
        client.VerifySaveState<MailAction<User>>(Stores.MailActions);
        emailWorker.Verify(x => x.RenderAndSendAsync("register-confirmation", request.RegisteringUser, It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}