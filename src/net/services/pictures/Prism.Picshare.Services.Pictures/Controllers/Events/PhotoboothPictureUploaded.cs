// -----------------------------------------------------------------------
//  <copyright file = "PhotoboothPictureUploaded.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Pictures.Commands.Albums;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Events;

public class PhotoboothPictureUploaded : Controller
{
    private readonly ILogger<PhotoboothPictureUploaded> _logger;
    private readonly IMediator _mediator;

    public PhotoboothPictureUploaded(ILogger<PhotoboothPictureUploaded> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost(Topics.Photobooth.Picture.Uploaded)]
    [Topic(Publishers.PubSub, Topics.Photobooth.Picture.Uploaded)]
    public async Task<IActionResult> Handle([FromBody] PhotoboothPicture photoboothPicture)
    {
        _logger.LogInformation("Process photobooth picture upload : {pictureId}", photoboothPicture.Id);

        await _mediator.Send(new InitializePicture(photoboothPicture.OrganisationId, photoboothPicture.Id, PictureSource.Photobooth));
        await _mediator.Send(new SetPictureName(photoboothPicture.OrganisationId, photoboothPicture.Id, photoboothPicture.OriginalFileName ?? string.Empty));
        await _mediator.Send(new AddPictureToAlbum(photoboothPicture.OrganisationId, photoboothPicture.SessionId, photoboothPicture.Id));

        return Ok();
    }
}