// -----------------------------------------------------------------------
//  <copyright file = "ConfigController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Prism.Picshare.Services.Api.Controllers;

public class ConfigController : Controller
{
    [HttpGet]
    [Route("api/config/insights")]
    [AllowAnonymous]
    public IActionResult Insights()
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

        var dbConnectionStringBuilder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        return Ok(new
        {
            instrumentationKey = dbConnectionStringBuilder["InstrumentationKey"].ToString(),
            connectionString
        });
    }
}