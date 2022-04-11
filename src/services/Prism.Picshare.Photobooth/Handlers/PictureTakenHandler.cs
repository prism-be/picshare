// -----------------------------------------------------------------------
//  <copyright file="PictureTakenHandler.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Data;
using Prism.Picshare.Events;
using Prism.Picshare.Photobooth.Commands;
using Prism.Picshare.Photobooth.Model;

namespace Prism.Picshare.Photobooth.Handlers;

public class PictureTakenHandler : IRequestHandler<PictureTaken>
{
    private readonly IDatabaseResolver _databaseResolver;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<PictureTakenHandler> _logger;

    public PictureTakenHandler(IDatabaseResolver databaseResolver, IEventPublisher eventPublisher, ILogger<PictureTakenHandler> logger)
    {
        _databaseResolver = databaseResolver;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public Task<Unit> Handle(PictureTaken request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing a picture taken request: {request}", request);

        using var db = _databaseResolver.GetDatabase(request.Organisation, DatabaseTypes.Photobooth);

        var picture = new Pictures(request.Id, request.Session, DateTime.UtcNow);
        db.Insert(picture);

        _eventPublisher.Publish(Topics.Photobooth.PictureTaken, request);

        return Task.FromResult(Unit.Value);
    }
}