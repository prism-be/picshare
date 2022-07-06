// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Prism.Picshare.Insights;

public static class LoggingBuilderExtensions
{
    public static void AddInsights(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ITelemetryInitializer, PicshareTelemetryInitializer>();

        builder.Services.AddApplicationInsightsTelemetry(opt =>
        {
            opt.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
        });

        builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft.Hosting.Lifetime", LogLevel.Information);
        builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
    }
}