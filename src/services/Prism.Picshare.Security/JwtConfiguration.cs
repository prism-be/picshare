// -----------------------------------------------------------------------
//  <copyright file="JwtConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Security;

public class JwtConfiguration
{
    public JwtConfiguration() { }

    public JwtConfiguration(string? key, string? audience, string? issuer)
    {
        Key = key;
        Audience = audience;
        Issuer = issuer;
    }

    public string? Audience { get; set; }

    public string? Issuer { get; set; }

    public string? Key { get; set; }
}