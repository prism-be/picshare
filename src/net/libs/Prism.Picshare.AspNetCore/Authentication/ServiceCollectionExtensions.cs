// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Security;

namespace Prism.Picshare.AspNetCore.Authentication;

public static class ServiceCollectionExtensions
{
    public static void AddPicshareAuthentication(this IServiceCollection services)
    {
        var config = new JwtConfiguration
        {
            PrivateKey = EnvironmentConfiguration.GetConfiguration("JWT_PRIVATE_KEY") ?? string.Empty,
            PublicKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PUBLIC_KEY")
        };
        services.AddSingleton(config);

        services.AddAuthentication(options => options.DefaultScheme = AuthSchemeConstants.PicshareAuthenticationScheme)
            .AddScheme<PicshareAuthenticationScheme, PicshareAuthenticationHandler>(AuthSchemeConstants.PicshareAuthenticationScheme, _ => { });

        services.AddScoped<IUserContextAccessor, UserContextAccessor>();
    }
}