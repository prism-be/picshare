// -----------------------------------------------------------------------
//  <copyright file="ServiceCollectionExtensions.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Events.Exceptions;
using RabbitMQ.Client;

namespace Prism.Picshare.Events.Rabbit;

public static class ServiceCollectionExtensions
{
    public static void AddRabbitMqExchange(this IServiceCollection services, Action<RabbitConfiguration> configure)
    {
        var configuration = new RabbitConfiguration();
        configure(configuration);

        if (string.IsNullOrWhiteSpace(configuration.Exchange))
        {
            throw new EventsConfigurationException("The event architecture is not configured : missing Exchange");
        }

        if (configuration.Uri == null)
        {
            throw new EventsConfigurationException("The event architecture is not configured : missing Uri");
        }

        services.AddSingleton(configuration);

        var factory = new ConnectionFactory { Uri = configuration.Uri };
        services.AddSingleton<IConnectionFactory>(factory);

        services.AddScoped<IEventPublisher, RabbitPublisher>();
    }
}