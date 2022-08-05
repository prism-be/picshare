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
        var messagesQueues = new Queue<ServiceBusMessage>(data.Select(item => JsonSerializer.Serialize(item)).Select(json => new ServiceBusMessage(json)));

        var messages = new List<ServiceBusMessage>();

        var client = new ServiceBusClient(EnvironmentConfiguration.GetMandatoryConfiguration("SERVICE_BUS_CONNECTION_STRING"));
        var sender = client.CreateSender(topic);

        while (messagesQueues.Count > 0)
        {
            messages.Add(messagesQueues.Dequeue());

            if (messages.Count == 20)
            {
                await sender.SendMessagesAsync(messages, cancellationToken);
                messages.Clear();
            }
        }

        if (messages.Count > 0)
        {
            await sender.SendMessagesAsync(messages, cancellationToken);
            messages.Clear();
        }
    }
}