// -----------------------------------------------------------------------
//  <copyright file = "CleanThumbnails.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures;

public record CleanThumbnails(Guid OrganisationId, Guid PictureId) : IRequest;

public class CleanThumbnailsHandler : IRequestHandler<CleanThumbnails>
{
    private readonly BlobClient _blobClient;
    private readonly ILogger<CleanThumbnailsHandler> _logger;

    public CleanThumbnailsHandler(ILogger<CleanThumbnailsHandler> logger, BlobClient blobClient)
    {
        _blobClient = blobClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(CleanThumbnails request, CancellationToken cancellationToken)
    {
        var blobs = await _blobClient.ListAsync(request.OrganisationId, request.PictureId, cancellationToken);

        foreach (var blob in blobs)
        {
            if (blob.EndsWith("/source.jpg"))
            {
                continue;
            }

            _logger.LogInformation("Deleting blob: {blob}", blob);
            await _blobClient.DeleteAsync(blob, cancellationToken);
        }
        
        return Unit.Value;
    }
}