// -----------------------------------------------------------------------
//  <copyright file = "PictureCreatedController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Processor.Commands;

namespace Prism.Picshare.Services.Processor.Controllers.Events;

public class PictureCreatedController: Controller
{
    private readonly IMediator _mediator;

    public PictureCreatedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.Created)]
    [Topic(Publishers.PubSub, Topics.Pictures.Created)]
    public async Task<IActionResult> Handle([FromBody] Picture picture)
    {
        await _mediator.Send(new ReadMetaData(picture.OrganisationId, picture.Id));

        return Ok();
    }
}