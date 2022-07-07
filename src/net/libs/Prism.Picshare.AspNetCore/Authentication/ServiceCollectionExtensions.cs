// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Prism.Picshare.AspNetCore.Authentication;

public static class ServiceCollectionExtensions
{
    public static void AddPicshareAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options => options.DefaultScheme = AuthSchemeConstants.PicshareAuthenticationScheme)
            .AddScheme<PicshareAuthenticationScheme, PicshareAuthenticationHandler>(AuthSchemeConstants.PicshareAuthenticationScheme, _ => { });
    }
}