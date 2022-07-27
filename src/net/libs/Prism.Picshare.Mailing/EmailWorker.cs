// -----------------------------------------------------------------------
//  <copyright file = "EmailWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Net.Mail;
using DotLiquid;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Mailing;

public interface IEmailWorker
{
    Task RenderAndSendAsync<T>(string template, User recipient, T data, CancellationToken cancellationToken);
}

public class EmailWorker : IEmailWorker
{
    private readonly ISmtpClientWrapper _smtpClient;
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(ILogger<EmailWorker> logger, ISmtpClientWrapper smtpClient)
    {
        _smtpClient = smtpClient;
        _logger = logger;
    }

    public async Task RenderAndSendAsync<T>(string template, User recipient, T data, CancellationToken cancellationToken)
    {
        var assemblyDirectory = Path.GetDirectoryName(typeof(EmailWorker).Assembly.Location);
        Debug.Assert(assemblyDirectory != null, nameof(assemblyDirectory) + " != null");
        var filePath = Path.Combine(assemblyDirectory, "Templates", $"{recipient.Culture}-{template}.txt");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Cannot found a template {template}, fall back to the default language", template);
            filePath = Path.Combine(assemblyDirectory, "Templates", $"en-{template}.txt");
        }
        
        if (!File.Exists(filePath))
        {
            _logger.LogCritical("Cannot found a template {template}, even in the default language", template);
            throw new FileNotFoundException(filePath);
        }

        var templateContent = await File.ReadAllTextAsync(filePath, cancellationToken);
        var splitContent = templateContent.Split('\n');
        var title = splitContent[0].Trim('\r');
        var body = string.Join('\n', splitContent.Skip(1));

        var titleTemplate = Template.Parse(title);
        var bodyTemplate = Template.Parse(body);

        title = titleTemplate.Render(Hash.FromAnonymousObject(data));
        body = bodyTemplate.Render(Hash.FromAnonymousObject(data));
        
        var message = new MailMessage("no-reply@picshare.me", recipient.Email, title, body);
        message.IsBodyHtml = false;

        await _smtpClient.SendAsync(message, cancellationToken);
    }
}