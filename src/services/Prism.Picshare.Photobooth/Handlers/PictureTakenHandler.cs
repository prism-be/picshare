// -----------------------------------------------------------------------
//  <copyright file="PictureTakenHandler.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Data;
using Prism.Picshare.Photobooth.Commands;
using Prism.Picshare.Photobooth.Model;

namespace Prism.Picshare.Photobooth.Handlers;

public class PictureTakenHandler : IRequestHandler<PictureTaken>
{
    private readonly IDatabaseResolver _databaseResolver;

    public PictureTakenHandler(IDatabaseResolver databaseResolver)
    {
        _databaseResolver = databaseResolver;
    }

    public Task<Unit> Handle(PictureTaken request, CancellationToken cancellationToken)
    {
        var db = _databaseResolver.GetDatabase(request.Organisation, DatabaseTypes.Photobooth);

        var picture = new Pictures(request.Id, request.Session, request.DateTaken);
        db.Insert(picture);

        return Task.FromResult(Unit.Value);
    }
}