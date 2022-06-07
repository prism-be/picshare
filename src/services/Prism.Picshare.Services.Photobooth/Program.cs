// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Services.Photobooth.Commands;
using Prism.Picshare.Services.Photobooth.Services;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddDaprClient();

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<PictureWatcher>();

var app = builder.Build();

app.UseStaticFiles();
app.UseHealthChecks("/health");

// Register routes
app.MapPost("api/take", async ([FromBody] PictureTaken request, IMediator mediator) => Results.Ok(await mediator.Send(request)));

// Let's run it !
app.Run();