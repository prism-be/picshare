// -----------------------------------------------------------------------
//  <copyright file = "PicturesController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Web;
using HttpMultipartParser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Extensions;
using Prism.Picshare.Security;

namespace Prism.Picshare.Services.Api.Controllers;

public class PicturesController : Controller
{
    private readonly BlobClient _blobClient;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<PicturesController> _logger;
    private readonly IMediator _mediator;
    private readonly StoreClient _storeClient;
    private readonly IUserContextAccessor _userContextAccessor;

    public PicturesController(IUserContextAccessor userContextAccessor, StoreClient storeClient, IMediator mediator, JwtConfiguration jwtConfiguration, ILogger<PicturesController> logger,
        BlobClient blobClient)
    {
        _userContextAccessor = userContextAccessor;
        _storeClient = storeClient;
        _mediator = mediator;
        _jwtConfiguration = jwtConfiguration;
        _logger = logger;
        _blobClient = blobClient;
    }

    [HttpGet]
    [Route("api/pictures/flow")]
    public async Task<IActionResult> Flow()
    {
        var flow = await _storeClient.GetStateAsync<Flow>(_userContextAccessor.OrganisationId.ToString());
        var authorizations = await _storeClient.GetStateAsync<Authorizations>(_userContextAccessor.OrganisationId, _userContextAccessor.Id);

        flow.Pictures = flow.Pictures.Where(x => x.Ready).ToList();

        foreach (var picture in flow.Pictures)
        {
            if (authorizations.Pictures.TryGetValue(picture.Id, out var token))
            {
                picture.Token = token;
            }
        }

        return Ok(flow);
    }

    [HttpGet]
    [Route("api/pictures/show/{organisationId:guid}/{pictureId:guid}")]
    public async Task<IActionResult> Show([FromRoute] Guid organisationId, [FromRoute] Guid pictureId)
    {
        if (!_userContextAccessor.HasAccess(organisationId))
        {
            return NotFound();
        }

        var picture = await _mediator.Send(new IncreaseViewCount(organisationId, pictureId));
        return Ok(picture);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("api/pictures/thumbs/{token}/{width:int}/{height:int}")]
    public async Task<IActionResult> Thumbs([FromRoute] string token, [FromRoute] int width, [FromRoute] int height)
    {
        var principal = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, token, _logger, false);

        if (principal == null)
        {
            _logger.LogInformation("Invalid token to access picture");
            return NotFound();
        }

        var organisationId = Guid.Parse(principal.Claims.Single(x => x.Type == ClaimsNames.OrganisationId).Value);
        var pictureId = Guid.Parse(principal.Claims.Single(x => x.Type == ClaimsNames.PictureId).Value);

        try
        {
            var blobName = BlobNamesExtensions.GetSourcePath(organisationId, pictureId, width, height);
            var data = await _blobClient.ReadAsync(blobName);

            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            Response?.Headers.Add("Cache-Control", "public, max-age=31536000");
            return File(data, "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cannot access to picture: CANNOT access to picture {pictureId} in organisation {orgId}",
                pictureId, organisationId);
        }

        return NotFound();
    }

    [HttpPost]
    [Route("api/pictures/upload")]
    public async Task<IActionResult> Upload()
    {
        var parsedFormBody = await MultipartFormDataParser.ParseAsync(Request.Body);

        foreach (var file in parsedFormBody.Files)
        {
            var pictureId = Identifier.Generate();
            _logger.LogInformation("Processing file uploaded : {fileName} to {pictureId} for {organisationId}", file.FileName, pictureId, _userContextAccessor.OrganisationId);

            using var memoryStream = new MemoryStream();
            await file.Data.CopyToAsync(memoryStream);
            var data = memoryStream.ToArray();

            var organisationId = _userContextAccessor.OrganisationId;
            var userId = _userContextAccessor.Id;
            await _mediator.Send(new UploadPicture(organisationId, pictureId, data));
            await _mediator.Send(new InitializePicture(organisationId, userId, pictureId, PictureSource.Upload));
            await _mediator.Send(new SetPictureName(organisationId, pictureId, HttpUtility.HtmlEncode(file.FileName)));
        }

        return NoContent();
    }
}