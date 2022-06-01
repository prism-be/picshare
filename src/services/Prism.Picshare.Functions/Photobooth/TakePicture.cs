// -----------------------------------------------------------------------
//  <copyright file="TakePicture.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Activities;
using Prism.Picshare.Functions.Extensions;

namespace Prism.Picshare.Photobooth;

public static class TakePicture
{
    [FunctionName(nameof(Photobooth) + nameof(TakePicture) + nameof(RunAsync))]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "photobooth/take-picture")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        var organisationId = req.GetOrganisationId();

        if (!Guid.TryParse(req.Query["albumId"], out var albumId))
        {
            return new BadRequestObjectResult("The album id is not well formatted");
        }

        var pictureId = Guid.NewGuid();

        var pictureReference = new PhotoBoothPictureTaken(organisationId, pictureId, albumId);
        await starter.StartNewAsync(nameof(Photobooth) + nameof(TakePicture) + nameof(RunOrchestrator), pictureReference);

        log.LogInformation("New picture taken from Photobooth for {organisationId} in {albumId} with reference {pictureId}", organisationId, albumId, pictureId);

        return new OkObjectResult(new
        {
            pictureId
        });
    }

    [FunctionName(nameof(Photobooth) + nameof(TakePicture) + nameof(RunOrchestrator))]
    public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var pictureReference = context.GetInput<PhotoBoothPictureTaken>();

        await context.CallActivityAsync(nameof(Activities) + nameof(Pictures) + nameof(Pictures.Create), pictureReference);

        return new List<string>();
    }
}