// -----------------------------------------------------------------------
//  <copyright file = "ThumbnailsGeneratedController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Controllers;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class ThumbnailsGeneratedController : Controller, IEventExecutor<EntityReference>
{
    private readonly IMediator _mediator;

    public ThumbnailsGeneratedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.ThumbnailsGenerated)]
    [Topic(Publishers.PubSub, Topics.Pictures.ThumbnailsGenerated)]
    public async Task<IActionResult> Execute([FromBody] EntityReference picture)
    {
        var summary = await _mediator.Send(new SetPictureReady(picture.OrganisationId, picture.Id));
        return Ok(summary);
    }
}