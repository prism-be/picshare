// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Photobooth.Commands;
using Prism.Picshare.Services.Photobooth.Hubs;
using Prism.Picshare.Services.Photobooth.Services;

var builder = WebApplication.CreateBuilder(args);

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddDaprClient();

builder.Services.AddHealthChecks();

builder.Services.AddHostedService<PictureWatcher>();

builder.Services.AddSignalR();

var originsConfiguration = builder.Configuration.GetSection("AllowedOrigins")?.Value?.Split(",");
var originsEnvironement = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",");

var origins = new List<string>();

if (originsConfiguration != null)
{
    origins.AddRange(originsConfiguration);
}

if (originsEnvironement != null)
{
    origins.AddRange(originsEnvironement);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(origins.ToArray())
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("ClientPermission");

app.MapSubscribeHandler();
app.UseCloudEvents();

app.UseHealthChecks("/health");

// Direct API
app.MapPost("/take", async ([FromBody] PictureTaken request, IMediator mediator)
    => Results.Ok(await mediator.Send(request)));

app.MapGet("/pictures/{pictureId:guid}", ([FromRoute] Guid pictureId, IHostEnvironment env)
    => Results.File(Path.Combine(env.ContentRootPath, "wwwroot", "pictures", pictureId.ToString())));

// SignalR
app.MapHub<PhotoboothHub>("/hubs/photobooth");

// PubSub Events
app.MapPost(Topics.Photobooth.PictureTaken,
    [Topic(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken)]
    async ([FromBody] PhotoboothPicture picture, IMediator mediator)
        => Results.Ok(await mediator.Send(new NotifyPictureTaken(picture))));

app.MapPost(Topics.Photobooth.PictureUploaded,
    [Topic(DaprConfiguration.PubSub, Topics.Photobooth.PictureUploaded)]
    async ([FromBody] PhotoboothPicture picture, IMediator mediator)
        => Results.Ok(await mediator.Send(new NotifyPictureUploaded(picture))));

// Let's run it !
await app.RunAsync();