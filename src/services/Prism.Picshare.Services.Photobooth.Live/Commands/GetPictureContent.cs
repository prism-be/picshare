// -----------------------------------------------------------------------
//  <copyright file = "GetPictureContent.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;

namespace Prism.Picshare.Services.Photobooth.Live.Commands;

public record GetPictureContent(Guid OrganisationId, Guid PictureId) : IRequest<byte[]?>;

public class GetPictureContentValidator : AbstractValidator<GetPictureContent>
{
    public GetPictureContentValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.PictureId).NotEmpty();
    }
}

public class GetPictureContentValidatorHandler : IRequestHandler<GetPictureContent, byte[]?>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<GetPictureContentValidatorHandler> _logger;

    public GetPictureContentValidatorHandler(DaprClient daprClient, ILogger<GetPictureContentValidatorHandler> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public async Task<byte[]?> Handle(GetPictureContent request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing request : {request}", request);

        var bindingRequest = new BindingRequest(DaprConfiguration.DataStore, DaprConfiguration.BindingOperation.Get);
        bindingRequest.Metadata.Add("blobName", $"{request.OrganisationId}/{request.PictureId}/source");
        bindingRequest.Metadata.Add("fileName", $"{request.OrganisationId}/{request.PictureId}/source");

        var response = await _daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);

        if (response == null)
        {
            _logger.LogInformation("No picture data found for the request : {request}", request);
            return null;
        }

        return response.Data.ToArray();
    }
}