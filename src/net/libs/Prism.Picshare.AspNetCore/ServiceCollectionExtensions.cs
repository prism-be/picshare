// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Security;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;
using StackExchange.Redis;

namespace Prism.Picshare.AspNetCore;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddDisconnectedPicshareServices(this IServiceCollection services)
    {
        // Add Mediatr
        var applicationAssembly = typeof(EntryPoint).Assembly;
        services.AddMediatR(applicationAssembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
        services.AddValidatorsFromAssembly(applicationAssembly);

        // Add the Jwt Config
        var jwtConfiguration = new JwtConfiguration
        {
            PrivateKey = EnvironmentConfiguration.GetConfiguration("JWT_PRIVATE_KEY") ?? string.Empty,
            PublicKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PUBLIC_KEY")
        };
        services.AddSingleton(jwtConfiguration);

        // Add the database
        var cosmosClient = new CosmosClient(EnvironmentConfiguration.GetMandatoryConfiguration("COSMOS_CONNECTION_STRING"));
        var database = cosmosClient.GetDatabase("picshare");
        services.AddSingleton(cosmosClient);
        services.AddSingleton(database);

        // Add the clients
        services.AddScoped<BlobClient, AzureBlobClient>();
        services.AddScoped<StoreClient, CosmosStoreClient>();
        services.AddScoped<PublisherClient, ServiceBusPublisherClient>();

        return services;
    }

    public static IServiceCollection AddPicshare(this IServiceCollection services)
    {
        services.AddDisconnectedPicshareServices();

        // Add the cache
        var redisConnection = ConnectionMultiplexer.Connect(EnvironmentConfiguration.GetMandatoryConfiguration("REDIS_CONNECTION_STRING"));
        var cache = redisConnection.GetDatabase();
        services.AddSingleton(cache);
        services.AddSingleton<RedisLocker>();

        return services;
    }
}