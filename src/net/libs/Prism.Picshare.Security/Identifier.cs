// -----------------------------------------------------------------------
//  <copyright file = "Security.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Prism.Picshare.Security;

public static class Identifier
{
    public static Guid Generate()
    {
        return new Guid(RandomNumberGenerator.GetBytes(16));
    }
}