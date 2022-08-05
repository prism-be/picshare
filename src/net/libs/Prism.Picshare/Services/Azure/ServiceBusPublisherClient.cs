// -----------------------------------------------------------------------
//  <copyright file = "ServiceBusPublisherClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace Prism.Picshare.Services.Azure;

public class ServiceBusPublisherClient : PublisherClient
{
    public override async Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default)
    {
        var client = new ServiceBusClient(EnvironmentConfiguration.GetMandatoryConfiguration("SERVICE_BUS_CONNECTION_STRING"));
        var sender = client.CreateSender(topic);

        var json = JsonSerializer.Serialize(data);
        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message, cancellationToken);
    }

    public override async Task PublishEventsAsync<T>(string topic, IEnumerable<T> data, CancellationToken cancellationToken = default)
    {
        var messages = data.Select(item => JsonSerializer.Serialize(item)).Select(json => new ServiceBusMessage(json));

        var client = new ServiceBusClient(EnvironmentConfiguration.GetMandatoryConfiguration("SERVICE_BUS_CONNECTION_STRING"));
        var sender = client.CreateSender(topic);
        await sender.SendMessagesAsync(messages, cancellationToken);
    }
}