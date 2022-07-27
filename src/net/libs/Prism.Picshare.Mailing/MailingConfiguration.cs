// -----------------------------------------------------------------------
//  <copyright file = "MailingConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Mailing;

public class MailingConfiguration
{
    public MailingConfiguration()
    {
        RootUri = string.Empty;
        SmtpPort = 587;
        SmtpUser = string.Empty;
        SmtpPassword = string.Empty;
        SmtpServer = string.Empty;
    }

    public int SmtpPort { get; set; }
    public string RootUri { get; set; }
    public string SmtpPassword { get; set; }
    public string SmtpServer { get; set; }
    public string SmtpUser { get; set; }
}