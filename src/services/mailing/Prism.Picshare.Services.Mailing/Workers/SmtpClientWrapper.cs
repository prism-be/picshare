// -----------------------------------------------------------------------
//  <copyright file = "SmtpClientWrapper.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Mail;

namespace Prism.Picshare.Services.Mailing.Workers;

public interface ISmtpClientWrapper
{
    Task SendAsync(MailMessage message, CancellationToken cancellationToken);
}

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly MailingConfiguration _configuration;

    public SmtpClientWrapper(MailingConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(MailMessage message, CancellationToken cancellationToken)
    {
        var client = new SmtpClient(_configuration.SmtpServer, _configuration.SmtpPort);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(_configuration.SmtpUser, _configuration.SmtpPassword);
        await client.SendMailAsync(message, cancellationToken);
    }
}