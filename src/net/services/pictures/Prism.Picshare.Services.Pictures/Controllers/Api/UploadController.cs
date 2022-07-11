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
        var pictureId = Security.GenerateIdentifier();
        _logger.LogInformation("Processing file uploaded : {fileName} to {pictureId} for {organisationId}", file.FileName, pictureId, _userContextAccessor.OrganisationId);

        await using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var data = memoryStream.ToArray();

        await _mediator.Send(new UploadPicture(_userContextAccessor.OrganisationId, pictureId, data));
        await _mediator.Send(new InitializePicture(_userContextAccessor.OrganisationId, pictureId, PictureSource.Upload));
        await _mediator.Send(new SetPictureName(_userContextAccessor.OrganisationId, pictureId, HttpUtility.HtmlEncode(file.FileName)));

        return Ok();
    }
}