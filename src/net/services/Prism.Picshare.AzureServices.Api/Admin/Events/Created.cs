// -----------------------------------------------------------------------
//  <copyright file = "Created.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Commands.Pictures.Admin;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Api.Admin.Events;

public class Created
{
    private readonly IMediator _mediator;

    public Created(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [Function(nameof(Admin) + "." + nameof(Events) + "." + nameof(Created))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pictures/admin/events/created")] HttpRequestData req,
        FunctionContext executionContext)
    {
        await _mediator.Send(new RelaunchPictureEvents(executionContext.GetUserContext().OrganisationId, Topics.Pictures.Created));

        return req.CreateResponse(HttpStatusCode.OK);
    }
}