// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Prism.Picshare;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Insights;
using Prism.Picshare.Mailing;
using Prism.Picshare.Security;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;
using Prism.Picshare.Services.Workers.Workers.Email;
using Prism.Picshare.Services.Workers.Workers.Pictures;
using Prism.Picshare.Services.Workers.Workers.User;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddInsights();

var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

var jwtConfiguration = new JwtConfiguration
{
    PrivateKey = EnvironmentConfiguration.GetConfiguration("JWT_PRIVATE_KEY") ?? string.Empty,
    PublicKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PUBLIC_KEY")
};
builder.Services.AddSingleton(jwtConfiguration);

var cosmosClient = new CosmosClient(EnvironmentConfiguration.GetMandatoryConfiguration("COSMOS_CONNECTION_STRING"));
var database = cosmosClient.GetDatabase("picshare");
builder.Services.AddSingleton(cosmosClient);
builder.Services.AddSingleton(database);

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

builder.Services.AddScoped<BlobClient, AzureBlobClient>();
builder.Services.AddScoped<StoreClient, CosmosStoreClient>();
builder.Services.AddScoped<PublisherClient, ServiceBusPublisherClient>();

builder.Services.AddHttpContextAccessor();
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