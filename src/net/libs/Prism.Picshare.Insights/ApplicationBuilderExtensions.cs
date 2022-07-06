// -----------------------------------------------------------------------
//  <copyright file = "ApplicationBuilderExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Prism.Picshare.Insights.Middlewares;

namespace Prism.Picshare.Insights;

public static class ApplicationBuilderExtensions
{
    public static void UseExceptionLogger(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ErrorLoggingMiddleware>();
    }
}