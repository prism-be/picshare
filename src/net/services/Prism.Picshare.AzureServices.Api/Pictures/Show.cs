// -----------------------------------------------------------------------
//  <copyright file = "Show.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Commands.Pictures;

namespace Prism.Picshare.AzureServices.Api.Pictures;

public class Show
{
    private readonly IMediator _mediator;

    public Show(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [Function(nameof(Pictures) + "." + nameof(Show))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "pictures/show/{organisationId}/{pictureId}")] HttpRequestData req,
        FunctionContext executionContext, Guid organisationId, Guid pictureId)
    {
        if (!executionContext.GetUserContext().HasAccess(organisationId))
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        var picture = await _mediator.Send(new IncreaseViewCount(organisationId, pictureId));
        return await req.CreateResponseAsync(HttpStatusCode.OK, picture);
    }
}