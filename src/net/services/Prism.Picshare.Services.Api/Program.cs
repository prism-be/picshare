// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare;
using Prism.Picshare.AspNetCore;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Insights;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddInsights();

builder.Services.AddPicshare();

builder.Services.AddHttpContextAccessor();
builder.Services.AddInsights();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddPicshareAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddCors();

var app = builder.Build();

app.UseCors(opt =>
{
    opt.AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(
            EnvironmentConfiguration.GetMandatoryConfiguration("FRONT_DOMAIN_FQDN"),
            EnvironmentConfiguration.GetMandatoryConfiguration("FRONT_DOMAIN_CUSTOM")
        );
});

app.UseAuthentication()
    .UseAuthorization();

app.UseExceptionLogger();

app.UseHealthChecks("/health");

app.MapControllers()
    .RequireAuthorization();

app.Run();