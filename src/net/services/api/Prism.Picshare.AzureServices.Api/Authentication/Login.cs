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

public class Login
{
    private readonly IMediator _mediator;

    public Login(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Authentication) + "." + nameof(Login))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "authentication/login")] HttpRequestData req, FunctionContext executionContext)
    {
        var request = await req.ReadFromJsonAsync<AuthenticationRequest>();

        if (request == null)
        {
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var result = await _mediator.Send(request);

        if (result == ResultCodes.Ok)
        {
            var token = await _mediator.Send(new GenerateTokenRequest(request.Login));
            
            if (token != null)
            {
                return await req.CreateResponseAsync(HttpStatusCode.OK, token);
            }

            return req.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        return req.CreateResponse(HttpStatusCode.Unauthorized);
    }
}