// -----------------------------------------------------------------------
//  <copyright file = "DaprClientExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Prism.Picshare.Dapr;

namespace Prism.Picshare.Extensions;

public static class DaprClientExtensions
{
    public static async Task<BindingResponse> ReadPictureAsync(this DaprClient daprClient, string blobName, CancellationToken cancellationToken)
    {
        var bindingRequest = new BindingRequest(Stores.Data, "get");
        bindingRequest.Metadata.Add("blobName", blobName);
        bindingRequest.Metadata.Add("fileName", blobName);

        return await daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);
    }
}