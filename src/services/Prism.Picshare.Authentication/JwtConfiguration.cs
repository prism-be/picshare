// -----------------------------------------------------------------------
//  <copyright file="JwtConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Authentication;

public class JwtConfiguration
{
    public string? Audiance { get; set; }

    public string? Issuer { get; set; }

    public string? Key { get; set; }
}