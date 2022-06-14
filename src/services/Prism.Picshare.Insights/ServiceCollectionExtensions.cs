// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Prism.Picshare.Insights;

public static class ServiceCollectionExtensions
{
    public static void AddInsights(this IServiceCollection services)
    {
        services.AddSingleton<ITelemetryInitializer, PicshareTelemetryInitializer>();
        
        services.AddApplicationInsightsTelemetry(opt =>
        {
            opt.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        });
    }

    public static void AddInsights(this ILoggingBuilder builder)
    {
        builder.AddApplicationInsights();
    }
}