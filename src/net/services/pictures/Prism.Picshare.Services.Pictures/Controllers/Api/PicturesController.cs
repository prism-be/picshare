// -----------------------------------------------------------------------
//  <copyright file = "PicturesController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class PicturesController : Controller
{
    private readonly IMediator _mediator;
    private readonly IUserContextAccessor _userContextAccessor;

    public PicturesController(IMediator mediator, IUserContextAccessor userContextAccessor)
    {
        _mediator = mediator;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [Route("api/pictures/show/{organisationId:guid}/{pictureId:guid}")]
    public async Task<IActionResult> GetPicture(Guid organisationId, Guid pictureId)
    {
        if (!_userContextAccessor.HasAccess(organisationId))
        {
            return NotFound();
        }
        
        var picture = await _mediator.Send(new IncreaseViewCount(organisationId, pictureId));
        return Ok(picture);
    }
}