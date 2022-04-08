// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Data.LiteDB;
using Prism.Picshare.Photobooth;
using Prism.Picshare.Photobooth.Commands;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register all services

var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.UseLiteDbStorage(config =>
{
    config.DatabaseDirectory = Environment.GetEnvironmentVariable("PICSHARE_DB_DIRECTORY");
    config.DatabaseDirectory = Environment.GetEnvironmentVariable("PICSHARE_DB_PASSWORD");
});

// Register routes
app.MapPost("api/take", async ([FromBody] PictureTaken user, IMediator mediator) =>
{
    await mediator.Send(user);
});

// Let's run it !
app.Run();