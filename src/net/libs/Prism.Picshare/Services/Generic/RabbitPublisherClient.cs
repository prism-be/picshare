// -----------------------------------------------------------------------
//  <copyright file = "RabbitPublisherClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Events;
using RabbitMQ.Client;

namespace Prism.Picshare.Services.Generic;

public class RabbitPublisherClient : PublisherClient
{
    private readonly IModel _channel;
    private readonly ILogger<RabbitPublisherClient> _logger;

    public RabbitPublisherClient(ILogger<RabbitPublisherClient> logger, IModel channel)
    {
        _logger = logger;
        _channel = channel;
    }

    public override Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing event on topic {topic}", topic);
        var json = JsonSerializer.Serialize(data);
        var payload = Encoding.Default.GetBytes(json);
        _channel.BasicPublish(topic, Topics.Subscription , null, payload);
        return Task.CompletedTask;
    }

    public override Task PublishEventsAsync<T>(string topic, IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        var enumerated = data.ToList();
        _logger.LogInformation("Publishing events ({count}) on topic {topic}", enumerated.Count, topic);
        var batch = _channel.CreateBasicPublishBatch();

        foreach (var item in enumerated)
        {
            var json = JsonSerializer.Serialize(item);
            var payload = Encoding.Default.GetBytes(json);
            batch.Add(topic, Topics.Subscription, false, null, new ReadOnlyMemory<byte>(payload));
        }

        batch.Publish();
        return Task.CompletedTask;
    }
}