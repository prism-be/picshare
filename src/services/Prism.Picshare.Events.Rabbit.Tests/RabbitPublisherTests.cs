// -----------------------------------------------------------------------
//  <copyright file="RabbitPublisherTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text;
using System.Text.Json;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Prism.Picshare.Events.Rabbit.Tests;

public class RabbitPublisherTests
{
    [Fact]
    public void Publish_Ok()
    {
        // Arrange
        var topic = Guid.NewGuid().ToString();

        var configuration = new RabbitConfiguration();
        configuration.Exchange = Guid.NewGuid().ToString();

        var channel = new Mock<IModel>();
        var connection = new Mock<IConnection>();
        connection.Setup(x => x.CreateModel()).Returns(channel.Object);
        var connectionFactory = new Mock<IConnectionFactory>();
        connectionFactory.Setup(x => x.CreateConnection()).Returns(connection.Object);

        var publisher = new RabbitPublisher(configuration, connectionFactory.Object);

        // Act
        var message = new { Name = "John", Code = "007" };
        publisher.Publish(topic, message);

        // Assert
        channel.Verify(x => x.ExchangeDeclare(configuration.Exchange, ExchangeType.Topic, false, false, null), Times.Once);
        channel.Verify(x => x.BasicPublish(configuration.Exchange, topic, It.IsAny<bool>(), null, It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
    }


}