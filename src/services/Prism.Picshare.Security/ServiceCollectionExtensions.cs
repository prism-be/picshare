// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
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
    }
}