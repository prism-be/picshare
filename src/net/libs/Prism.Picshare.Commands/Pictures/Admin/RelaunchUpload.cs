// -----------------------------------------------------------------------
//  <copyright file = "RelaunchUpload.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures.Admin;

public record RelaunchUpload(Guid OrganisationId) : IRequest;

public class RelaunchUploadHandler : IRequestHandler<RelaunchUpload>
{
    private readonly BlobClient _blobClient;
    private readonly PublisherClient _publisherClient;

    public RelaunchUploadHandler(BlobClient blobClient, PublisherClient publisherClient)
    {
        _blobClient = blobClient;
        _publisherClient = publisherClient;
    }

    public async Task<Unit> Handle(RelaunchUpload request, CancellationToken cancellationToken)
    {
        var items = await _blobClient.ListAsync(request.OrganisationId, cancellationToken);

        foreach (var path in items.Where(x => x.EndsWith("/source.jpg")))
        {
            var pathSplitted = path.Split('/');
            var organisationId = Guid.Parse(pathSplitted[^3]);
            var pictureId = Guid.Parse(pathSplitted[^2]);

            await _publisherClient.PublishEventAsync(Topics.Pictures.Uploaded, new EntityReference
            {
                OrganisationId = organisationId,
                Id = pictureId
            }, cancellationToken);
        }

        return Unit.Value;
    }
}