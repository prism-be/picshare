// -----------------------------------------------------------------------
//  <copyright file = "JwtConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services.Authentication.Configuration;

public class JwtConfiguration
{
    public JwtConfiguration()
    {
        PrivateKey = PublicKey = string.Empty;
    }

    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
}