// -----------------------------------------------------------------------
//  <copyright file = "Flow.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Services;

namespace Prism.Picshare.AzureServices.Api.Pictures;

public class Flow
{
    private readonly StoreClient _storeClient;

    public Flow(StoreClient storeClient)
    {
        _storeClient = storeClient;
    }

    [Authorize]
    [Function(nameof(Pictures) + "." + nameof(Flow))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pictures/flow")] HttpRequestData req, FunctionContext executionContext)
    {
        var flow = await _storeClient.GetStateAsync<Domain.Flow>(executionContext.GetUserContext().OrganisationId.ToString());

        flow.Pictures = flow.Pictures.Where(x => x.Ready).ToList();

        return await req.CreateResponseAsync(HttpStatusCode.OK, flow);
    }
}