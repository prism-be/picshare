// -----------------------------------------------------------------------
//  <copyright file="PortraitOrchestrator.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Photobooth.Commands;

namespace Prism.Picshare.Functions.Photobooth;

public static class PortraitOrchestrator
{
    [FunctionName("PortraitOrchestrator")]
    public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        
        var outputs = new List<string>();

        // Replace "hello" with the name of your Durable Activity Function.
        outputs.Add(await context.CallActivityAsync<string>("PortraitOrchestrator_Hello", "Tokyo"));
        outputs.Add(await context.CallActivityAsync<string>("PortraitOrchestrator_Hello", "Seattle"));
        outputs.Add(await context.CallActivityAsync<string>("PortraitOrchestrator_Hello", "London"));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [FunctionName("PortraitOrchestrator_Hello")]
    public static async Task<string> SayHello([ActivityTrigger] string name, ILogger log)
    {
        log.LogInformation($"Saying hello to {name}.");
        return $"Hello {name}!";
    }

    [FunctionName("PortraitOrchestrator_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        var pictureTaken = req.Content.ReadAsAsync<PictureTaken>();

        if (pictureTaken == null)
        {
            log.LogError("Please post an object of type {object} in body.", nameof(pictureTaken));
            return req.CreateErrorResponse(HttpStatusCode.BadRequest, $"Please post an object of type {nameof(pictureTaken)} in body.");
        }

        // Function input comes from the request content.
        var instanceId = await starter.StartNewAsync("PortraitOrchestrator", pictureTaken);

        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}