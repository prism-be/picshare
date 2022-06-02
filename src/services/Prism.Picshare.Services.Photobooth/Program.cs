// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Services.Photobooth.Commands;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddDaprClient();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHealthChecks("/health");

// Register routes
app.MapPost("api/setup", async ([FromBody] Setup setup, IMediator mediator) => Results.Ok(await mediator.Send(setup)));
app.MapPost("api/take", async ([FromBody] PictureTaken request, IMediator mediator) => Results.Ok(await mediator.Send(request)));

// Let's run it !
app.Run();