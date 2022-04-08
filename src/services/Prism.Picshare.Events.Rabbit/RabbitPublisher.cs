// -----------------------------------------------------------------------
//  <copyright file="RabbitPublisher.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Prism.Picshare.Events.Rabbit;

public class RabbitPublisher: IEventPublisher
{
    private readonly RabbitConfiguration _rabbitConfiguration;
    private readonly IConnectionFactory _factory;

    public RabbitPublisher(RabbitConfiguration rabbitConfiguration, IConnectionFactory factory)
    {
        _rabbitConfiguration = rabbitConfiguration;
        _factory = factory;
    }

    public void Publish<T>(string topic, T message)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(_rabbitConfiguration.Exchange, ExchangeType.Topic);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(_rabbitConfiguration.Exchange, topic, body: body);
    }
}