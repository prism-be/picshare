// -----------------------------------------------------------------------
//  <copyright file = "Security.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Prism.Picshare;

public static class Security
{
    public static Guid GenerateIdentifier()
    {
        return new Guid(RandomNumberGenerator.GetBytes(16));
    }
}