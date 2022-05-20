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

    private readonly Container _container;

    public PictureRepository(CosmosClient cosmosClient)
    {
        _container = cosmosClient.GetDatabase(Database).GetContainer(Container);
    }

    public async Task Upsert(Guid organisationId, Picture picture)
    {
        picture.OrganisationId = organisationId;
        await _container.UpsertItemAsync(picture);
    }

    public async Task<Picture?> Get(Guid organisationId, Guid pictureId)
    {
        var result = await this._container.ReadItemAsync<Picture>(pictureId.ToString(), new PartitionKey(organisationId.ToString()));
        return result.StatusCode == HttpStatusCode.OK ? result.Resource : default;
    }
}