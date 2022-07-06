// -----------------------------------------------------------------------
//  <copyright file = "DaprClientExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Configuration;

namespace Prism.Picshare.Services.Pictures.Commands.Albums;

public static class DaprClientExtensions
{

    public static async Task<Album> GetStateAlbumAsync(this DaprClient daprClient, Guid organisationId, Guid pictureId, CancellationToken cancellationToken)
    {
        var key = EntityReference.ComputeKey(organisationId, pictureId);
        return await daprClient.GetStateAsync<Album>(Stores.Albums, key, cancellationToken: cancellationToken)
               ?? new Album
               {
                   OrganisationId = organisationId,
                   Id = pictureId
               };
    }

    public static async Task SaveStateAsync(this DaprClient daprClient, Album album, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>
        {
            {
                "partitionKey", album.OrganisationId.ToString()
            }
        };

        await daprClient.SaveStateAsync(Stores.Albums, album.Key, album, cancellationToken: cancellationToken, metadata: metadata);
    }
}