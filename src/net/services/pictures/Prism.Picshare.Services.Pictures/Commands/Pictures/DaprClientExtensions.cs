// -----------------------------------------------------------------------
//  <copyright file = "DaprClientExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Pictures.Configuration;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public static class DaprClientExtensions
{

    public static async Task<Picture> GetStatePictureAsync(this DaprClient daprClient, Guid organisationId, Guid pictureId, CancellationToken cancellationToken)
    {
        var key = EntityReference.ComputeKey(organisationId, pictureId);
        return await daprClient.GetStateAsync<Picture>(Stores.Pictures, key, cancellationToken: cancellationToken)
               ?? new Picture
               {
                   OrganisationId = organisationId,
                   Id = pictureId
               };
    }

    public static async Task SaveStateAsync(this DaprClient daprClient, Picture picture, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>
        {
            {
                "partitionKey", picture.OrganisationId.ToString()
            }
        };

        await daprClient.SaveStateAsync(Stores.Pictures, picture.Key, picture, cancellationToken: cancellationToken, metadata: metadata);
    }
}