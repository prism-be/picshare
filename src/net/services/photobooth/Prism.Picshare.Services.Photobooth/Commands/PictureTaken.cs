// -----------------------------------------------------------------------
//  <copyright file="PictureTaken.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Photobooth.Commands;

public record PictureTaken(Guid OrganisationId, Guid SessionId) : IRequest<PhotoboothPicture>;

public class PictureTakenValidator : AbstractValidator<PictureTaken>
{
    public PictureTakenValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}

public class PictureTakenHandler : IRequestHandler<PictureTaken, PhotoboothPicture>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<PictureTakenHandler> _logger;

    public PictureTakenHandler(ILogger<PictureTakenHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<PhotoboothPicture> Handle(PictureTaken request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing a picture taken request: {request}", request);

        var photoboothPicture = new PhotoboothPicture
        {
            Id = Guid.NewGuid(),
            OrganisationId = request.OrganisationId,
            SessionId = request.SessionId
        };

        await _daprClient.PublishEventAsync(DaprConfiguration.PubSub, Topics.Photobooth.PictureTaken, photoboothPicture, cancellationToken);

        return photoboothPicture;
    }
}