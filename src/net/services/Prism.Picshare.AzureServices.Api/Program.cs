// -----------------------------------------------------------------------
//  <copyright file = "Program.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Picshare.AzureServices.Middlewares;
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Security;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;

namespace Prism.Picshare.AzureServices.Api;

internal static class Program
{
    private static async Task Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults((_, builder) =>
            {
                builder.UseMiddleware<AuthenticationMiddleware>();
            })
            .ConfigureServices(services =>
            {
                var applicationAssembly = typeof(EntryPoint).Assembly;
                services.AddMediatR(applicationAssembly);
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
                services.AddValidatorsFromAssembly(applicationAssembly);

                var jwtConfiguration = new JwtConfiguration
                {
                    PrivateKey = EnvironmentConfiguration.GetConfiguration("JWT_PRIVATE_KEY") ?? string.Empty,
                    PublicKey = EnvironmentConfiguration.GetMandatoryConfiguration("JWT_PUBLIC_KEY")
                };
                services.AddSingleton(jwtConfiguration);
                
                var cosmosClient = new CosmosClient(EnvironmentConfiguration.GetMandatoryConfiguration("COSMOS_CONNECTION_STRING"));
                var database = cosmosClient.GetDatabase("picshare");
                services.AddSingleton(cosmosClient);
                services.AddSingleton(database);

                services.AddScoped<BlobClient, AzureBlobClient>();
                services.AddScoped<StoreClient, CosmosStoreClient>();
                services.AddScoped<PublisherClient, ServiceBusPublisherClient>();
            })
            .Build();

        await host.RunAsync();
    }
}