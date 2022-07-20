// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Prism.Picshare.AspNetCore;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Insights;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddInsights();

var applicationAssembly = typeof(Program).Assembly;
builder.Services.AddMediatR(applicationAssembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);

builder.Services.AddPicshareDependencies();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddPicshareAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication()
    .UseAuthorization();

app.MapSubscribeHandler();
app.UseCloudEvents();
app.UseHealthChecks("/health");

app.MapControllers()
    .RequireAuthorization();

app.Run();