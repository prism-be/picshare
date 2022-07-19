﻿// -----------------------------------------------------------------------
//  <copyright file = "PictureUploadedController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Processor.Commands;

namespace Prism.Picshare.Services.Processor.Controllers.Events;

public class PictureUploadedController : Controller
{
    private readonly DaprClient _daprClient;
    private readonly IMediator _mediator;

    public PictureUploadedController(IMediator mediator, DaprClient daprClient)
    {
        _mediator = mediator;
        _daprClient = daprClient;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.Uploaded)]
    [Topic(Publishers.PubSub, Topics.Pictures.Uploaded)]
    public async Task<IActionResult> PictureUploaded([FromBody] EntityReference entityReference)
    {
        var resizeTasks = new List<Task>
        {
            _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 150, 150, true)),
            _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 960, 540, false)),
            _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 1920, 1080, false)),
            _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 3840, 2160, false))
        };

        Task.WaitAll(resizeTasks.ToArray());

        await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.ThumbnailsGenerated, new EntityReference
        {
            Id = entityReference.Id,
            OrganisationId = entityReference.OrganisationId
        }, CancellationToken.None);

        return Ok();
    }
}