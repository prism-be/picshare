// -----------------------------------------------------------------------
//  <copyright file = "Check.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Api.Contracts;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;

namespace Prism.Picshare.AzureServices.Api.Authentication.User;

public class Check
{
    [Authorize]
    [Function(nameof(Authentication) + "." + nameof(User) + "." + nameof(Check))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authentication/user/check")] HttpRequestData req, FunctionContext executionContext)
    {
        var userContext = executionContext.GetUserContext();

        if (userContext.IsAuthenticated)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new UserAuthentication
            {
                Authenticated = userContext.IsAuthenticated,
                Organisation = userContext.OrganisationId,
                Name = userContext.Name
            });
            return response;
        }

        var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
        await unauthorizedResponse.WriteAsJsonAsync(new UserAuthentication
        {
            Authenticated = false
        });
        return unauthorizedResponse;
    }
}