// -----------------------------------------------------------------------
//  <copyright file = "PictureUploadedController.cs" company = "Prism">
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

public class PictureUploadedController : Controller
{
    private readonly IMediator _mediator;

    public PictureUploadedController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.Uploaded)]
    [Topic(Publishers.PubSub, Topics.Pictures.Uploaded)]
    public async Task<IActionResult> PictureUploaded([FromBody] EntityReference entityReference)
    {
        await _mediator.Send(new GenerateThumbnail(entityReference.OrganisationId, entityReference.Id, 150, 150));
        return Ok();
    }
}