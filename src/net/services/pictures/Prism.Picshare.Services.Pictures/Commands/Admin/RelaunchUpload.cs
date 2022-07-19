// -----------------------------------------------------------------------
//  <copyright file = "RelaunchUpload.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Admin;

public record RelaunchUpload(Guid OrganisationId) : IRequest;

public class RelaunchUploadHandler : IRequestHandler<RelaunchUpload>
{
    private readonly IBlobClient _blobClient;
    private readonly IPublisherClient _publisherClient;

    public RelaunchUploadHandler(IBlobClient blobClient, IPublisherClient publisherClient)
    {
        _blobClient = blobClient;
        _publisherClient = publisherClient;
    }

    public async Task<Unit> Handle(RelaunchUpload request, CancellationToken cancellationToken)
    {
        foreach (var path in await _blobClient.ListAsync(request.OrganisationId, cancellationToken))
        {
            if (path.EndsWith("/source.jpg"))
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
        }

        return Unit.Value;
    }
}