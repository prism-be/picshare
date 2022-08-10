// -----------------------------------------------------------------------
//  <copyright file = "PictureUpdated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers.Pictures;

public class PictureUpdated : BaseServiceBusWorker<Picture>
{
    private readonly ILogger<PictureUpdated> _logger;
    public PictureUpdated(ILogger<PictureUpdated> logger, IServiceProvider serviceProvider) : base(logger)
    {
        _logger = logger;
    }

    public override string Queue => Topics.Pictures.Updated;

    internal override Task ProcessMessageAsync(IMediator mediator, Picture payload)
    {
        _logger.LogInformation("Picture updated : {id}", payload.Id);
        _logger.LogDebug("Picture updated : {id}", payload);

        return Task.CompletedTask;
    }
}