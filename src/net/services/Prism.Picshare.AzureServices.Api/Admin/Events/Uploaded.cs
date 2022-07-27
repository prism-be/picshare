// -----------------------------------------------------------------------
//  <copyright file = "Uploaded.cs" company = "Prism">
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

namespace Prism.Picshare.AzureServices.Api.Admin.Events;

public class Uploaded
{
    private readonly IMediator _mediator;

    public Uploaded(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [Function(nameof(Admin) + "." + nameof(Events) + "." + nameof(Uploaded))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pictures/admin/events/uploaded")] HttpRequestData req,
        FunctionContext executionContext)
    {
        await _mediator.Send(new RelaunchUpload(executionContext.GetUserContext().OrganisationId));

        return req.CreateResponse(HttpStatusCode.OK);
    }
}