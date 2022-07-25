// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;
using Prism.Picshare.Services.Dapr;

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

        if (string.IsNullOrWhiteSpace(EnvironmentConfiguration.GetConfiguration("AZURE_BLOB_CONNECTION_STRING")))
        {
            services.AddTransient<BlobClient, DaprBlobClient>();
        }
        else
        {
            services.AddTransient<BlobClient, AzureBlobClient>();
        }

        services.AddTransient<StoreClient, DaprStoreClient>();
        services.AddTransient<PublisherClient, DaprPublisherClient>();
    }
}