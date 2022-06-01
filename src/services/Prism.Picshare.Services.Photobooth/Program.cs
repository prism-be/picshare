// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Data.CosmosDB;
using Prism.Picshare.Services.Photobooth.Commands;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.UseCosmosDB();

// Register routes
app.MapPost("api/take", async ([FromBody] PictureTaken user, IMediator mediator) =>
{
    await mediator.Send(user);
});

// Let's run it !
app.Run();