// -----------------------------------------------------------------------
//  <copyright file = "WebApplicationBuilderExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Prism.Picshare.AspNetCore;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder, string source)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{environment}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .WriteTo.Elasticsearch(ConfigureElasticSink(source))
            .Enrich.WithProperty("Environment", environment!)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        
        Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        
        builder.Host.UseSerilog();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(string source)
    {
        var elasticUri = EnvironmentConfiguration.GetMandatoryConfiguration("ELASTIC_URI");
        return new ElasticsearchSinkOptions(new Uri(elasticUri))
        {
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
            IndexFormat = $"logs-picshare-{source.ToLowerInvariant().Replace(".", "-")}",
            BatchAction = ElasticOpType.Create,
            TypeName = null
        };
    }
}