// -----------------------------------------------------------------------
//  <copyright file = "SmtpClientWrapper.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Prism.Picshare.Mailing;

public interface ISmtpClientWrapper
{
    Task SendAsync(MailMessage message, CancellationToken cancellationToken);
}

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly ILogger<SmtpClientWrapper> _logger;
    private readonly MailingConfiguration _configuration;

    public SmtpClientWrapper(MailingConfiguration configuration, ILogger<SmtpClientWrapper> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(MailMessage message, CancellationToken cancellationToken)
    {
        if (_configuration.SmtpServer == "EMPTY")
        {
            _logger.LogWarning("Smtp Server is not specified, no mail will be sent.");
            return;
        }
        
        var client = new SmtpClient(_configuration.SmtpServer, _configuration.SmtpPort);
        client.EnableSsl = true;
        client.Credentials = new NetworkCredential(_configuration.SmtpUser, _configuration.SmtpPassword);
        await client.SendMailAsync(message, cancellationToken);
    }
}