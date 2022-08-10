// -----------------------------------------------------------------------
//  <copyright file = "SharedInstances.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services.Workers.Workers;

public static class SharedInstances
{
    public static IServiceProvider ServiceProvider { get; set; } = null!;
}