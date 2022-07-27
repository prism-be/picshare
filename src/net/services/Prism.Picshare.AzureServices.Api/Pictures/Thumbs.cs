// -----------------------------------------------------------------------
//  <copyright file = "Thumbs.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Extensions;
using Prism.Picshare.Services;

namespace Prism.Picshare.AzureServices.Api.Pictures;

public class Thumbs
{
    private readonly BlobClient _blobClient;
    private readonly ILogger<Thumbs> _logger;

    public Thumbs(ILogger<Thumbs> logger, BlobClient blobClient)
    {
        _logger = logger;
        _blobClient = blobClient;
    }

    [Authorize]
    [Function(nameof(Pictures) + "." + nameof(Thumbs))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pictures/thumbs/{organisationId}/{pictureId}/{width}/{height}")] HttpRequestData req,
        FunctionContext executionContext, Guid organisationId, Guid pictureId, int width, int height)
    {
        var userContext = executionContext.GetUserContext();

        if (!userContext.HasAccess(organisationId))
        {
            _logger.LogInformation("User {userId} CANNOT access to picture {pictureId} in organisation {orgId} with authenticated org : {authOrgId}", userContext.Id, pictureId,
                organisationId, userContext.OrganisationId);
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

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
            _logger.LogWarning(ex, "Cannot access to picture: user {userId} CANNOT access to picture {pictureId} in organisation {orgId} with authenticated org : {authOrgId}",
                userContext.Id, pictureId, organisationId, userContext.OrganisationId);
        }

        return req.CreateResponse(HttpStatusCode.NotFound);
    }
}