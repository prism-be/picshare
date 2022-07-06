// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net.Mime;
using FluentValidation;
using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Insights;
using Prism.Picshare.Services.Photobooth.Live.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInsights();

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddDaprClient(config =>
{
    config.UseGrpcChannelOptions(new GrpcChannelOptions
    {
        MaxReceiveMessageSize = 30 * 1024 * 1024,
        MaxSendMessageSize = 30 * 1024 * 1024
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHealthChecks("/health");

app.MapGet("/taken/{organisationId:guid}/{sessionId:guid}/{pictureId:guid}",
    async ([FromRoute] Guid organisationId, [FromRoute] Guid sessionId, [FromRoute] Guid pictureId, IMediator mediator, ILogger<Program> logger)
        =>
    {
        var request = new GetPictureContent(organisationId, pictureId);
        logger.LogInformation("Processing request : {request}", request);
        
        var data = await mediator.Send(request);
        return data == null ? Results.NotFound() : Results.File(data, MediaTypeNames.Image.Jpeg);
    });

// Let's run it !
await app.RunAsync();