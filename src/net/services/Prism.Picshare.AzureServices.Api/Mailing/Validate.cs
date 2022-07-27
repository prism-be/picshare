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
using Prism.Picshare.Commands.Mailing;

namespace Prism.Picshare.AzureServices.Api.Mailing;

public class Validate
{
    private readonly IMediator _mediator;

    public Validate(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(AllowAnonymous = true)]
    [Function(nameof(Mailing) + "." + nameof(Validate))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "mailing/validate/{id}")] HttpRequestData req, FunctionContext executionContext, Guid id)
    {
        var result = await _mediator.Send(new RegisterConfirmationValidation(id));

        switch (result)
        {
            case ResultCodes.Ok:
                return await req.CreateResponseAsync(HttpStatusCode.OK, new
                {
                    code = ResultCodes.Ok
                });
            case ResultCodes.MailActionAlreadyConsumed:
            case ResultCodes.MailActionNotFound:
                return req.CreateResponse(HttpStatusCode.NotFound);
        }

        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
}