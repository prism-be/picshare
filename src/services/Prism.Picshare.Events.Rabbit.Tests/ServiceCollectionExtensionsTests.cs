// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensionsTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Events.Exceptions;
using RabbitMQ.Client;
using Xunit;

namespace Prism.Picshare.Events.Rabbit.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRabbitMqExchange_Ok()
    {
        // Arrange
        var exchange = Guid.NewGuid().ToString();
        var uri = new Uri("amqp://guest:guest@localhost:5672");

        // Act
        var services = new ServiceCollection();
        services.AddRabbitMqExchange(config =>
        {
            config.Exchange = exchange;
            config.Uri = uri;

        });

        // Assert
        Assert.Equal(1, services.Count(x => x.ServiceType == typeof(RabbitConfiguration)));
        Assert.Equal(1, services.Count(x => x.ServiceType == typeof(IConnectionFactory)));
        Assert.Equal(1, services.Count(x => x.ImplementationType == typeof(RabbitPublisher)));

        var config = services.Single(x => x.ServiceType == typeof(RabbitConfiguration)).ImplementationInstance as RabbitConfiguration;

        Assert.NotNull(config);
        Assert.Equal(exchange, config!.Exchange);
        Assert.Equal(uri, config.Uri);
    }

    [Fact]
    public void AddRabbitMqExchange_NoUri()
    {
        // Arrange
        var exchange = Guid.NewGuid().ToString();

        // Act
        var services = new ServiceCollection();
        var ex = Assert.Throws<EventsConfigurationException>(() => services.AddRabbitMqExchange(config =>
        {
            config.Exchange = exchange;

        }));

        // Assert
        Assert.Equal("The event architecture is not configured : missing Uri", ex.Message);
    }

    [Fact]
    public void AddRabbitMqExchange_NoExchange()
    {
        // Arrange
        var uri = new Uri("amqp://guest:guest@localhost:5672");

        // Act
        var services = new ServiceCollection();
        var ex = Assert.Throws<EventsConfigurationException>(() => services.AddRabbitMqExchange(config =>
        {
            config.Uri = uri;

        }));

        // Assert
        Assert.Equal("The event architecture is not configured : missing Exchange", ex.Message);
    }
}