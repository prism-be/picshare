// -----------------------------------------------------------------------
//  <copyright file = "Upload.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Web;
using HttpMultipartParser;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Security;

namespace Prism.Picshare.AzureServices.Api.Pictures;

public class Upload
{
    private readonly ILogger<Upload> _logger;
    private readonly IMediator _mediator;

    public Upload(ILogger<Upload> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [ExcludeFromCodeCoverage]
    [Authorize]
    [Function(nameof(Pictures) + "." + nameof(Upload))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "pictures/upload")] HttpRequestData req, FunctionContext executionContext)
    {
        var parsedFormBody = await MultipartFormDataParser.ParseAsync(req.Body);

        foreach (var file in parsedFormBody.Files)
        {
            var pictureId = Identifier.Generate();
            _logger.LogInformation("Processing file uploaded : {fileName} to {pictureId} for {organisationId}", file.FileName, pictureId, executionContext.GetUserContext().OrganisationId);

            using var memoryStream = new MemoryStream();
            await file.Data.CopyToAsync(memoryStream);
            var data = memoryStream.ToArray();

            var organisationId = executionContext.GetUserContext().OrganisationId;
            var userId = executionContext.GetUserContext().Id;
            await _mediator.Send(new UploadPicture(organisationId, pictureId, data));
            await _mediator.Send(new InitializePicture(organisationId, userId, pictureId, PictureSource.Upload));
            await _mediator.Send(new SetPictureName(organisationId, pictureId, HttpUtility.HtmlEncode(file.FileName)));
        }

        return req.CreateResponse(HttpStatusCode.OK);
    }
}