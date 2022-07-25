// -----------------------------------------------------------------------
//  <copyright file = "ExifReadController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Dapr;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class ExifReadController : Controller
{
    private readonly IMediator _mediator;

    public ExifReadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost(Topics.RoutePrefix + Topics.Pictures.ExifRead)]
    [Topic(Publishers.PubSub, Topics.Pictures.ExifRead)]
    public async Task<IActionResult> Execute([FromBody] Picture picture)
    {
        picture = await _mediator.Send(new GeneratePictureSummary(picture.OrganisationId, picture.Id, picture.Exifs));
        return Ok(picture);
    }
}