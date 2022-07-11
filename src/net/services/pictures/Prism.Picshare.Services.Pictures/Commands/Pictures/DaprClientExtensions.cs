// -----------------------------------------------------------------------
//  <copyright file = "DaprClientExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public static class DaprClientExtensions
{

    public static async Task<Flow> GetStateFlowAsync(this DaprClient daprClient, Guid organisationId, CancellationToken cancellationToken)
    {
        var flow = await daprClient.GetStateAsync<Flow>(Stores.Flow, organisationId.ToString(), cancellationToken: cancellationToken);

        if (flow == null)
        {
            flow = new Flow
            {
                OrganisationId = organisationId
            };

            await daprClient.SaveStateAsync(flow, cancellationToken);
        }

        return flow;
    }

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

    public static async Task SaveStateAsync(this DaprClient daprClient, Flow flow, CancellationToken cancellationToken)
    {
        await daprClient.SaveStateAsync(Stores.Flow, flow.OrganisationId.ToString(), flow, cancellationToken: cancellationToken);
    }
}