// -----------------------------------------------------------------------
//  <copyright file = "ExifRead.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Prism.Picshare.Commands.Pictures;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.AzureServices.Workers.Pictures;

public class ExifRead
{
    private readonly IMediator _mediator;

    public ExifRead(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function(nameof(Pictures) + "." + nameof(ExifRead))]
    public async Task Run([ServiceBusTrigger(Topics.Pictures.ExifRead, Connection = "SERVICE_BUS_CONNECTION_STRING")] string mySbMsg, FunctionContext context)
    {
        var picture = JsonSerializer.Deserialize<Picture>(mySbMsg);

        if (picture == null)
        {
            return;
        }

        await _mediator.Send(new GeneratePictureSummary(picture.OrganisationId, picture.Id, picture.Exifs));
    }
}