// -----------------------------------------------------------------------
//  <copyright file = "Insights.cs" company = "Prism">
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

public class Register
{
    private readonly IMediator _mediator;

    public Register(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Authentication) + "." + nameof(Register))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/register")] HttpRequestData req, FunctionContext executionContext)
    {
        var request = await req.ReadFromJsonAsync<RegisterAccountRequest>();

        if (request == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var responseCode = await _mediator.Send(request);

        if (responseCode == ResultCodes.Ok)
        {
            return req.CreateResponse(HttpStatusCode.NoContent);
        }
        
        return await req.CreateResponseAsync(HttpStatusCode.Conflict, new
        {
            code = responseCode
        });
    }
}