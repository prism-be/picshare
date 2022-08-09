// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Security;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;
using Prism.Picshare.Services.Generic;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Prism.Picshare.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDisconnectedPicshareServices(this IServiceCollection services)
    {
        // Add Mediatr
        var applicationAssembly = typeof(EntryPoint).Assembly;
        services.AddMediatR(new [] { applicationAssembly}, config => config.AsScoped());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
        services.AddValidatorsFromAssembly(applicationAssembly);

        // Add the Jwt Config
        var jwtConfiguration = new JwtConfiguration
        {
            PrivateKey = EnvironmentConfiguration.GetConfiguration("JWT_PRIVATE_KEY") ?? string.Empty,
            PublicKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PUBLIC_KEY")
        };
        services.AddSingleton(jwtConfiguration);

        services.AddStore();
        services.AddPublisher();

        // Add the clients
        services.AddTransient<BlobClient, AzureBlobClient>();
        services.AddSingleton<RedisLocker>();

        return services;
    }

    public static void AddPicshare(this IServiceCollection services)
    {
        services.AddDisconnectedPicshareServices();

        // Add the cache
        var redisConnection = ConnectionMultiplexer.Connect(EnvironmentConfiguration.GetMandatoryConfiguration("REDIS_CONNECTION_STRING"));
        var cache = redisConnection.GetDatabase();
        services.AddSingleton(cache);
    }

    private static void AddPublisher(this IServiceCollection services)
    {
        var serviceBusConnectionString = EnvironmentConfiguration.GetConfiguration("SERVICE_BUS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(serviceBusConnectionString))
        {
            services.AddTransient<PublisherClient, ServiceBusPublisherClient>();
        }

        var rabbitMqConnectionString = EnvironmentConfiguration.GetConfiguration("RABBITMQ_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(rabbitMqConnectionString))
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqConnectionString)
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            services.AddSingleton(channel);
            services.AddSingleton<PublisherClient, RabbitPublisherClient>();
        }
    }

    private static void AddStore(this IServiceCollection services)
    {
        var cosmosConnectionString = EnvironmentConfiguration.GetConfiguration("COSMOS_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(cosmosConnectionString))
        {
            var cosmosClient = new CosmosClient(cosmosConnectionString);
            var database = cosmosClient.GetDatabase("picshare");
            services.AddSingleton(cosmosClient);
            services.AddSingleton(database);
            services.AddTransient<StoreClient, CosmosStoreClient>();
        }

        var liteDbConnectionString = EnvironmentConfiguration.GetConfiguration("LITE_DB_DIRECTORY");

        if (!string.IsNullOrWhiteSpace(liteDbConnectionString))
        {
            services.AddTransient<StoreClient, LiteDbStoreClient>();
        }

        var mongoDbConnectionString = EnvironmentConfiguration.GetConfiguration("MONGODB_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(mongoDbConnectionString))
        {
            var client = new MongoClient(mongoDbConnectionString);
            services.AddSingleton<IMongoClient>(client);
            services.AddTransient<StoreClient, MongoStoreClient>();
        }
    }
}