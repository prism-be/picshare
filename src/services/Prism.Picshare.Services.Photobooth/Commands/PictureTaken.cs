// -----------------------------------------------------------------------
//  <copyright file="PictureTaken.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Photobooth.Commands;

public record PictureTaken(Guid OrganisationId, Guid SessionId) : IRequest<Guid>;

public class PictureTakenValidator : AbstractValidator<PictureTaken>
{
    public PictureTakenValidator()
    {
        this.RuleFor(x => x.OrganisationId).NotEmpty();
        this.RuleFor(x => x.SessionId).NotEmpty();
    }
}

public class PictureTakenHandler : IRequestHandler<PictureTaken, Guid>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<PictureTakenHandler> _logger;

    public PictureTakenHandler(ILogger<PictureTakenHandler> logger, DaprClient daprClient)
    {
        this._logger = logger;
        this._daprClient = daprClient;
    }

    public async Task<Guid> Handle(PictureTaken request, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Processing a picture taken request: {request}", request);

        var pictureCreated = new PicureCreated(Guid.NewGuid(), request.OrganisationId, request.SessionId);
        await this._daprClient.PublishEventAsync(PubSub.Pictures, PicureCreated.Topic, pictureCreated, cancellationToken);

        return pictureCreated.Id;
    }
}