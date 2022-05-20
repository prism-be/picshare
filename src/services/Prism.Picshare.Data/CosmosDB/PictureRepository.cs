// -----------------------------------------------------------------------
//  <copyright file="PictureRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data.CosmosDB;

public class PictureRepository : IPictureRepository
{
    public const string Database = "picshare";
    public const string Container = "pictures";

    public Task<int> Upsert(Guid organisationId, Picture picture)
    {
        throw new NotImplementedException();
    }

    public Task<Picture> Get(Guid organisationId, Guid pictureId)
    {
        throw new NotImplementedException();
    }
}