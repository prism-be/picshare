// -----------------------------------------------------------------------
//  <copyright file = "EmailValidated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.User;

public class EmailValidated
{
    [Function(nameof(User) + "." + nameof(EmailValidated))]
    public void Run([ServiceBusTrigger(Topics.Email.Validated, Topics.Subscription, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var logger = context.GetLogger("EmailValidated");
        logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
    }
}