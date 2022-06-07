// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

var app = builder.Build();

app.MapSubscribeHandler();
app.UseCloudEvents();

app.UseStaticFiles();
app.UseHealthChecks("/health");

app.MapPost("/take", async ([FromBody] PictureTaken request, IMediator mediator) => Results.Ok(await mediator.Send(request)));

app.MapHub<PhotoboothHub>("/hubs/photobooth");

app.MapPost(Topics.Photobooth.PictureTaken, [Topic(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken)] async ([FromBody]PhotoboothPicture picture, IHubContext<PhotoboothHub> hubContext) =>
{
    await hubContext.Clients.All.SendAsync("PictureTaken", picture);
    
    Console.WriteLine("Picture Received received : " + picture);
    return Results.Ok(picture);
});

// Let's run it !
await app.RunAsync();