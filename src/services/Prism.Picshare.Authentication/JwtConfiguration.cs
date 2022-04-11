// -----------------------------------------------------------------------
//  <copyright file="JwtConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Authentication;

public record JwtConfiguration (string Key, string Audience, string Issuer);