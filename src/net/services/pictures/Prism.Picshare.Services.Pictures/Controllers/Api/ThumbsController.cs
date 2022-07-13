// -----------------------------------------------------------------------
//  <copyright file = "ThumbsController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Extensions;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class ThumbsController : Controller
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<ThumbsController> _logger;
    private readonly IUserContextAccessor _userContextAccessor;

    public ThumbsController(ILogger<ThumbsController> logger, IUserContextAccessor userContextAccessor, DaprClient daprClient)
    {
        _daprClient = daprClient;
        _logger = logger;
        _userContextAccessor = userContextAccessor;
    }

    [HttpGet]
    [Route("api/pictures/thumbs/{organisationId:guid}/{pictureId:guid}/{width:int}/{height:int}")]
    public async Task<IActionResult> GetThumbs([FromRoute] Guid organisationId, [FromRoute] Guid pictureId, [FromRoute] int width, [FromRoute] int height)
    {
        if (organisationId != _userContextAccessor.OrganisationId)
        {
            _logger.LogInformation("User {userId} CANNOT access to picture {pictureId} in organisation {orgId} with authenticated org : {authOrgId}", _userContextAccessor.Id, pictureId,
                organisationId, _userContextAccessor.OrganisationId);
            return NotFound();
        }

        try
        {
            var blobName = BlobNamesExtensions.GetSourcePath(organisationId, pictureId, width, height);
            var data = await _daprClient.ReadPictureAsync(blobName, new CancellationToken());
            return File(data.Data.ToArray(), "image/jpeg");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cannot access to picture: user {userId} CANNOT access to picture {pictureId} in organisation {orgId} with authenticated org : {authOrgId}",
                _userContextAccessor.Id, pictureId, organisationId, _userContextAccessor.OrganisationId);
        }

        return NotFound();
    }
}