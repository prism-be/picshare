// -----------------------------------------------------------------------
//  <copyright file="PictureRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.CosmosDB;

public class PictureRepository : IPictureRepository
{
    public const string Database = "picshare";
    public const string Container = "pictures";

    private readonly CosmosClient _cosmosClient;

    public PictureRepository(CosmosClient cosmosClient)
    {
        this._cosmosClient = cosmosClient;
    }

    public Task<HttpStatusCode> Upsert(Guid organisationId, Picture picture)
    {
        var container = _cosmosClient.GetDatabase(Database).GetContainer(Container);
        throw new NotImplementedException();
    }

    public Task<Picture> Get(Guid organisationId, Guid pictureId)
    {
        throw new NotImplementedException();
    }
}