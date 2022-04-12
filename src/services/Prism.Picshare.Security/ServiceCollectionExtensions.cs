// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Prism.Picshare.Security.Exceptions;

namespace Prism.Picshare.Security;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, Action<JwtConfiguration> config)
    {
        var configuration = new JwtConfiguration();
        config(configuration);

        if (string.IsNullOrWhiteSpace(configuration.Key))
        {
            throw new JwtConfigurationException("Application cannot start because of missing variable: JwtConfiguration.Key");
        }

        if (string.IsNullOrWhiteSpace(configuration.Audience))
        {
            throw new JwtConfigurationException("Application cannot start because of missing variable: JwtConfiguration.Audience");
        }

        if (string.IsNullOrWhiteSpace(configuration.Issuer))
        {
            throw new JwtConfigurationException("Application cannot start because of missing variable: JwtConfiguration.Issuer");
        }

        services.AddSingleton(configuration);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.Issuer,
                    ValidAudience = configuration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Key))
                };
            });

        services.AddAuthorization();
    }
}