// -----------------------------------------------------------------------
//  <copyright file = "Refresh.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Commands.Authentication;

namespace Prism.Picshare.AzureServices.Api.Authentication;

public class Refresh
{
    private readonly IMediator _mediator;

    public Refresh(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Authentication) + "." + nameof(Refresh))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/refresh")] HttpRequestData req, FunctionContext executionContext)
    {
        var request = await req.ReadFromJsonAsync<RefreshTokenRequest>();

        if (request == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var token = await _mediator.Send(request);

        if (token != null)
        {
            return await req.CreateResponseAsync(HttpStatusCode.OK, token);
        }

        return req.CreateResponse(HttpStatusCode.Unauthorized);
    }
}