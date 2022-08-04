// -----------------------------------------------------------------------
//  <copyright file = "PictureCreated.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Workers.Workers;

public class PictureCreated : BackgroundService, IAsyncDisposable
{
    private readonly ILogger<PictureCreated> _logger;

    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public PictureCreated(ILogger<PictureCreated> logger)
    {
        _logger = logger;
    }

    public string Queue => Topics.Pictures.Created;

    

    
}