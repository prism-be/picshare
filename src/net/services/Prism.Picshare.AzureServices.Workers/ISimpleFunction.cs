// -----------------------------------------------------------------------
//  <copyright file = "ISimpleFunction.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;

namespace Prism.Picshare.AzureServices.Workers;

public interface ISimpleFunction
{
    Task Run(string mySbMsg, FunctionContext context);
}