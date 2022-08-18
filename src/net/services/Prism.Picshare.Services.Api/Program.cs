// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare;
using Prism.Picshare.AspNetCore;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Mailing;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog("api");

builder.Services.AddPicshare();

var mailingConfiguration = new MailingConfiguration
{
    RootUri = EnvironmentConfiguration.GetMandatoryConfiguration("ROOT_URI"),
};
builder.Services.AddSingleton(mailingConfiguration);
builder.Services.AddScoped<IEmailWorker, NullEmailWorker>();

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

app.UseHealthChecks("/health");

app.MapControllers()
    .RequireAuthorization();

app.Run();