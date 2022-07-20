﻿// -----------------------------------------------------------------------
//  <copyright file = "PictureUploadedController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Processor.Commands;

namespace Prism.Picshare.Services.Processor.Controllers.Events;

public class PictureUploadedController : Controller
{
    private readonly IMediator _mediator;
    private readonly PublisherClient _publisherClient;

    public PictureUploadedController(IMediator mediator, PublisherClient publisherClient)
    {
        _mediator = mediator;
        _publisherClient = publisherClient;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.Uploaded)]
    [Topic(Publishers.PubSub, Topics.Pictures.Uploaded)]
    public async Task<IActionResult> PictureUploaded([FromBody] EntityReference entityReference)
    {
        await _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 150, 150, true));
        await _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 960, 540, false));
        await _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 1920, 1080, false));
        await _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 3840, 2160, false));

        await _publisherClient.PublishEventAsync(Topics.Pictures.ThumbnailsGenerated, new EntityReference
        {
            Id = entityReference.Id,
            OrganisationId = entityReference.OrganisationId
        }, CancellationToken.None);

        return Ok();
    }
}