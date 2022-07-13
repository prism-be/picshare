// -----------------------------------------------------------------------
//  <copyright file = "IEventExecutor.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace Prism.Picshare.AspNetCore.Controllers;

public interface IEventExecutor<in T>
{
    Task<IActionResult> Execute(T request);
}