// -----------------------------------------------------------------------
//  <copyright file="Albums.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Data.CosmosDB;

namespace Prism.Picshare.Activities;

public class Albums
{
    private readonly Container _container;

    public Albums(CosmosClient cosmosClient)
    {
        this._container = cosmosClient.GetDatabase(DatabaseStructure.Pictures.Database).GetContainer(DatabaseStructure.Pictures.Container);
    }

    [FunctionName(nameof(Activities) + nameof(Albums) + nameof(Create))]
    public async Task<Guid> Create([ActivityTrigger] PictureReference pictureReference, ILogger log)
    {
        log.LogInformation("Create picture {pictureId} for {organisationId}", pictureReference.PictureId, pictureReference.OrganisationId);

        var picture = new Picture
        {
            Id = pictureReference.PictureId, OrganisationId = pictureReference.OrganisationId, Source = pictureReference.Source
        };

        await this._container.CreateItemAsync(picture);

        return pictureReference.PictureId;
    }
}