// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using Grpc.Net.Client;
using MediatR;
using Prism.Picshare;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Insights;
using Prism.Picshare.Services.Authentication.Configuration;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Logging.AddInsights();

builder.Services.AddDaprClient(config =>
{
    config.UseGrpcChannelOptions(new GrpcChannelOptions
    {
        MaxReceiveMessageSize = 30 * 1024 * 1024,
        MaxSendMessageSize = 30 * 1024 * 1024
    });
});

var config = new JwtConfiguration
{
    PrivateKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PRIVATE_KEY")
};
builder.Services.AddSingleton(config);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionLogger();

app.MapSubscribeHandler();
app.UseCloudEvents();
app.UseHealthChecks("/health");

app.MapControllers();

app.Run();