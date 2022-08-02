// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Prism.Picshare;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Insights;
using Prism.Picshare.Security;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;

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

builder.Services.AddScoped<BlobClient, AzureBlobClient>();
builder.Services.AddScoped<StoreClient, CosmosStoreClient>();
builder.Services.AddScoped<PublisherClient, ServiceBusPublisherClient>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddInsights();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddPicshareAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization();

app.UseExceptionLogger();

app.UseHealthChecks("/health");

app.MapControllers()
    .RequireAuthorization();

app.Run();