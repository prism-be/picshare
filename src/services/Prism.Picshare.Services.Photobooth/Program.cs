// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Events;
using Prism.Picshare.Events.Behaviors;
using Prism.Picshare.Events.Photobooth;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register all services

var applicationAssembly = typeof(EntryPoint).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

// Register routes
app.MapPost("api/take", async ([FromBody] PictureTaken user, IMediator mediator) =>
{
    await mediator.Send(user);
});

// Let's run it !
app.Run();