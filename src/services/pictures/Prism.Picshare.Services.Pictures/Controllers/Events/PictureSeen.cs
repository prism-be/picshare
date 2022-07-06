// -----------------------------------------------------------------------
//  <copyright file = "PictureSeen.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class PictureSeen : Controller
{
    private readonly ILogger<PictureSeen> _logger;
    private readonly IMediator _mediator;

    public PictureSeen(ILogger<PictureSeen> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost(Topics.Pictures.Seen)]
    [Topic(DaprConfiguration.PubSub, Topics.Pictures.Seen)]
    public async Task<IActionResult> Handle([FromBody] EntityReference picture)
    {
        _logger.LogInformation("Process picture seen : {pictureId}", picture.Id);

        await _mediator.Send(new IncreaseViewCount(picture.OrganisationId, picture.Id));

        return Ok();
    }
}