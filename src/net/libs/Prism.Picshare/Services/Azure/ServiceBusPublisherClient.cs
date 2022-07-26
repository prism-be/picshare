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
}