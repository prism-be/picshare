// -----------------------------------------------------------------------
//  <copyright file = "ServiceCollectionExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;

namespace Prism.Picshare.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static void AddPicshareDependencies(this IServiceCollection services)
    {
        services.AddTransient<BlobClient, AzureBlobClient>();
        services.AddTransient<StoreClient, CosmosStoreClient>();
        services.AddTransient<PublisherClient, ServiceBusPublisherClient>();
    }
}