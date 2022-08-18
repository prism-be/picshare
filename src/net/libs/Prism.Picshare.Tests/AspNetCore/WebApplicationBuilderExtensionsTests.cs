// -----------------------------------------------------------------------
//  <copyright file = "WebApplicationBuilderExtensionsTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Prism.Picshare.AspNetCore;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class WebApplicationBuilderExtensionsTests
{
    [Fact]
    public void AddSerilog_Ok()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        Environment.SetEnvironmentVariable("ELASTIC_URI", "http://localhost:9000");
        // Act
        builder.ConfigureSerilog("workers");

        // Assert
        builder.Services.SingleOrDefault(x => x.ServiceType == typeof(Serilog.Extensions.Hosting.DiagnosticContext))
            .Should().NotBeNull();
    }
}