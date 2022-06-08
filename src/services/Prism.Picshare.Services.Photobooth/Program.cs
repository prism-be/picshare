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
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Value.Split(","))
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("ClientPermission");

app.MapSubscribeHandler();
app.UseCloudEvents();

app.UseHealthChecks("/health");

app.MapPost("/take", async ([FromBody] PictureTaken request, IMediator mediator)
    => Results.Ok(await mediator.Send(request)));

app.MapGet("/pictures/{pictureId:guid}", ([FromRoute] Guid pictureId, IHostEnvironment env)
    => Results.File(Path.Combine(env.ContentRootPath, "wwwroot", "pictures", pictureId.ToString())));

app.MapHub<PhotoboothHub>("/hubs/photobooth");

app.MapPost(Topics.Photobooth.PictureTaken,
    [Topic(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken)]
    async ([FromBody] PhotoboothPicture picture, IMediator mediator)
        => Results.Ok(await mediator.Send(new NotifyPictureTaken(picture))));

// Let's run it !
await app.RunAsync();