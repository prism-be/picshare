// -----------------------------------------------------------------------
//  <copyright file = "PicshareTelemetryInitializer.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Prism.Picshare.Insights;

public class PicshareTelemetryInitializer : ITelemetryInitializer
{
    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_ROLE_NAME");
    }
}