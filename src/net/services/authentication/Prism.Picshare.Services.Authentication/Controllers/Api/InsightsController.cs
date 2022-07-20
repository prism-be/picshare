// -----------------------------------------------------------------------
//  <copyright file = "InsightsController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Prism.Picshare.Services.Authentication.Controllers.Api;

public class InsightsController : Controller
{
    [AllowAnonymous]
    [HttpGet("/api/authentication/insights")]
    public IActionResult GetConfiguration()
    {
        var connectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return Ok(new
            {
                instrumentationKey = string.Empty,
                connectionString = string.Empty
            });
        }

        DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        return Ok(new
        {
            instrumentationKey = dbConnectionStringBuilder["InstrumentationKey"].ToString(),
            connectionString = connectionString
        });
    }
}