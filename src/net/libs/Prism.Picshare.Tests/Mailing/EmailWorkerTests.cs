// -----------------------------------------------------------------------
//  <copyright file = "EmailWorkerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Mailing;
using Xunit;

namespace Prism.Picshare.Tests.Mailing;

public class EmailWorkerTests
{

    [Fact]
    public async Task RenderAndSend_Fallback_Default_Language()
    {
        // Arrange
        var smtpClient = new Mock<ISmtpClientWrapper>();
        var user = new User
        {
            Email = $"{Guid.NewGuid()}@picshare.me",
            Name = "Unit Test",
            Culture = "fr"
        };
        var data = new
        {
            name = "Unit Test"
        };
        var logger = new Mock<ILogger<EmailWorker>>();

        // Act
        var emailWorker = new EmailWorker(logger.Object, smtpClient.Object);
        await emailWorker.RenderAndSendAsync("unit-test", user, data, CancellationToken.None);

        // Assert
        smtpClient.Verify(x => x.SendAsync(It.Is<MailMessage>(m =>
            m.To.Any(t => t.Address == user.Email)
            && m.Subject == "Hello Unit Test !"
            && m.IsBodyHtml == false
            && m.Body == "This is the message content for Unit Test"
        ), default), Times.Once);
    }

    [Fact]
    public async Task RenderAndSend_No_Template()
    {
        // Arrange
        var smtpClient = new Mock<ISmtpClientWrapper>();
        var user = new User
        {
            Email = $"{Guid.NewGuid()}@picshare.me",
            Name = "Unit Test",
            Culture = "fr"
        };
        var data = new
        {
            name = "Unit Test"
        };
        var logger = new Mock<ILogger<EmailWorker>>();

        // Act and assert
        var emailWorker = new EmailWorker(logger.Object, smtpClient.Object);
        await Assert.ThrowsAsync<FileNotFoundException>(async () => await emailWorker.RenderAndSendAsync("unit-test-not-found", user, data, CancellationToken.None));
    }

    [Fact]
    public async Task RenderAndSend_Ok()
    {
        // Arrange
        var smtpClient = new Mock<ISmtpClientWrapper>();
        var user = new User
        {
            Email = $"{Guid.NewGuid()}@picshare.me",
            Name = "v"
        };
        var data = new
        {
            name = "Unit Test"
        };
        var logger = new Mock<ILogger<EmailWorker>>();

        // Act
        var emailWorker = new EmailWorker(logger.Object, smtpClient.Object);
        await emailWorker.RenderAndSendAsync("unit-test", user, data, CancellationToken.None);

        // Assert
        smtpClient.Verify(x => x.SendAsync(It.Is<MailMessage>(m =>
            m.To.Any(t => t.Address == user.Email)
            && m.Subject == "Hello Unit Test !"
            && m.IsBodyHtml == false
            && m.Body == "This is the message content for Unit Test"
        ), default), Times.Once);
    }
}