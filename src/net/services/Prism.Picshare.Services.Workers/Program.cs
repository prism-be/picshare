// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare;
using Prism.Picshare.AspNetCore;
using Prism.Picshare.Insights;
using Prism.Picshare.Mailing;
using Prism.Picshare.Services.Workers.Workers.Email;
using Prism.Picshare.Services.Workers.Workers.Pictures;
using Prism.Picshare.Services.Workers.Workers.User;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddInsights();

builder.Services.AddPicshare();

var mailingConfiguration = new MailingConfiguration
{
    RootUri = EnvironmentConfiguration.GetMandatoryConfiguration("ROOT_URI"),
    SmtpPort = Convert.ToInt32(EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PORT")),
    SmtpServer = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_SERVER"),
    SmtpUser = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_USER"),
    SmtpPassword = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PASSWORD")
};
builder.Services.AddSingleton(mailingConfiguration);

builder.Services.AddScoped<IEmailWorker, EmailWorker>();
builder.Services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();

builder.Services.AddInsights();

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<EmailValidated>();
builder.Services.AddHostedService<PictureCreated>();
builder.Services.AddHostedService<PictureExifRead>();
builder.Services.AddHostedService<PictureSeen>();
builder.Services.AddHostedService<PictureSummaryUpdated>();
builder.Services.AddHostedService<PictureThumbnailsGenerated>();
builder.Services.AddHostedService<PictureUploaded>();
builder.Services.AddHostedService<UserRegister>();

var app = builder.Build();

app.UseExceptionLogger();

app.UseHealthChecks("/health");

app.Run();