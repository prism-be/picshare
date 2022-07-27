// -----------------------------------------------------------------------
//  <copyright file = "AuthorizeAttribute.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.AzureServices.Middlewares;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute
{
    public bool AllowAnonymous { get; set; }
}