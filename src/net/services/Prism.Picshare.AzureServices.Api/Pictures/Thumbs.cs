// -----------------------------------------------------------------------
//  <copyright file = "Thumbs.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Extensions;
using Prism.Picshare.Security;
using Prism.Picshare.Services;

namespace Prism.Picshare.AzureServices.Api.Pictures;

public class Thumbs
{
    private readonly BlobClient _blobClient;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<Thumbs> _logger;

    public Thumbs(ILogger<Thumbs> logger, BlobClient blobClient, JwtConfiguration jwtConfiguration)
    {
        _logger = logger;
        _blobClient = blobClient;
        _jwtConfiguration = jwtConfiguration;
    }

    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Pictures) + "." + nameof(Thumbs))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pictures/thumbs/{token}/{width}/{height}")] HttpRequestData req,
        FunctionContext executionContext, string token, int width, int height)
    {
        var principal = TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, token, _logger, false);

        if (principal == null)
        {
            _logger.LogInformation("Invalid token to access picture");
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var organisationId = Guid.Parse(principal.Claims.Single(x => x.Type == ClaimsNames.OrganisationId).Value);
        var pictureId = Guid.Parse(principal.Claims.Single(x => x.Type == ClaimsNames.PictureId).Value);

        try
        {
            var blobName = BlobNamesExtensions.GetSourcePath(organisationId, pictureId, width, height);
            var data = await _blobClient.ReadAsync(blobName);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/jpeg");
            await response.WriteBytesAsync(data);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cannot access to picture: CANNOT access to picture {pictureId} in organisation {orgId}",
                pictureId, organisationId);
        }

        return req.CreateResponse(HttpStatusCode.NotFound);
    }
}