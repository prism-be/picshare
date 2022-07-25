// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Prism.Picshare;
using Prism.Picshare.AspNetCore;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Insights;
using Prism.Picshare.Services.Mailing;
using Prism.Picshare.Services.Mailing.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddInsights();

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

var configuration = new MailingConfiguration
{
    RootUri = EnvironmentConfiguration.GetMandatoryConfiguration("ROOT_URI"),
    SmtpPort = Convert.ToInt32(EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PORT")),
    SmtpServer = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_SERVER"),
    SmtpUser = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_USER"),
    SmtpPassword = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PASSWORD")
};
builder.Services.AddSingleton(configuration);
builder.Services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
builder.Services.AddScoped<IEmailWorker, EmailWorker>();

builder.Services.AddPicshareDependencies();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionLogger();

app.MapSubscribeHandler();
app.UseCloudEvents();
app.UseHealthChecks("/api/mailing/health");

app.MapControllers();

app.Run();