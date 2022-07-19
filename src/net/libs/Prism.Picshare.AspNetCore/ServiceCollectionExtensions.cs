// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Dapr;

namespace Prism.Picshare.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static void AddPicshareDependencies(this IServiceCollection services)
    {
        services.AddDaprClient(config =>
        {
            config.UseGrpcChannelOptions(new GrpcChannelOptions
            {
                MaxReceiveMessageSize = 30 * 1024 * 1024,
                MaxSendMessageSize = 30 * 1024 * 1024
            });
        });

        services.AddTransient<IBlobClient, BlobClient>();
        services.AddTransient<IStoreClient, StoreClient>();
        services.AddTransient<IPublisherClient, PublisherClient>();
    }
}