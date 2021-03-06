// -----------------------------------------------------------------------
//  <copyright file = "UserRegister.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Processor;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;

namespace Prism.Picshare.AzureServices.Workers.Pictures;

public class Uploaded
{
    private readonly IMediator _mediator;
    private readonly PublisherClient _publisherClient;

    public Uploaded(IMediator mediator, PublisherClient publisherClient)
    {
        _mediator = mediator;
        _publisherClient = publisherClient;
    }

    [Function(nameof(Pictures) + "." + nameof(Uploaded))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.Uploaded, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var picture = JsonSerializer.Deserialize<EntityReference>(mySbMsg);

        if (picture == null)
        {
            return;
        }

        await _mediator.Send(new GenerateThumbnail(picture.OrganisationId, picture.Id, 150, 150, true));
        await _mediator.Send(new GenerateThumbnail(picture.OrganisationId, picture.Id, 960, 540, false));
        await _mediator.Send(new GenerateThumbnail(picture.OrganisationId, picture.Id, 1920, 1080, false));
        await _mediator.Send(new GenerateThumbnail(picture.OrganisationId, picture.Id, 3840, 2160, false));

        await _publisherClient.PublishEventAsync(Topics.Pictures.ThumbnailsGenerated, new EntityReference
        {
            Id = picture.Id,
            OrganisationId = picture.OrganisationId
        }, CancellationToken.None);
    }
}