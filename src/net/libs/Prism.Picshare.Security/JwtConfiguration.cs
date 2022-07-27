// -----------------------------------------------------------------------
//  <copyright file = "JwtConfiguration.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Security;

public class JwtConfiguration
{
    public const string Audience = "picshare-front";
    public const string Issuer = "picshare-authentication";

    public JwtConfiguration()
    {
        PrivateKey = PublicKey = string.Empty;
    }

    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
}