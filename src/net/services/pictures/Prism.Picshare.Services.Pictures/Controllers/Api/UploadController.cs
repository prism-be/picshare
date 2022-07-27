// -----------------------------------------------------------------------
//  <copyright file = "UploadController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class UploadController : Controller
{
    private readonly ILogger<UploadController> _logger;
    private readonly IMediator _mediator;
    private readonly IUserContextAccessor _userContextAccessor;

    public UploadController(ILogger<UploadController> logger, IMediator mediator, IUserContextAccessor userContextAccessor)
    {
        _logger = logger;
        _mediator = mediator;
        _userContextAccessor = userContextAccessor;
    }

    [HttpPost]
    [Route("api/pictures/upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        
    }
}