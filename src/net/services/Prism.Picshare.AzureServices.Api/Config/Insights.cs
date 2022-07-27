// -----------------------------------------------------------------------
//  <copyright file = "Insights.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Data.Common;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Middlewares;

namespace Prism.Picshare.AzureServices.Api.Config;

public class Insights
{
    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Config) + "." + nameof(Insights))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config/insights")] HttpRequestData req, FunctionContext executionContext)
    {
        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        var response = req.CreateResponse(HttpStatusCode.OK);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            await response.WriteAsJsonAsync(new
            {
                instrumentationKey = string.Empty,
                connectionString = string.Empty
            });

            return response;
        }

        var dbConnectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        await response.WriteAsJsonAsync(new
        {
            instrumentationKey = dbConnectionStringBuilder["InstrumentationKey"].ToString(),
            connectionString
        });

        return response;
    }
}