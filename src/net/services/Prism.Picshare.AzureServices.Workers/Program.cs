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
using Prism.Picshare.Behaviors;
using Prism.Picshare.Commands;
using Prism.Picshare.Mailing;
using Prism.Picshare.Services;
using Prism.Picshare.Services.Azure;

namespace Prism.Picshare.AzureServices.Workers;

internal class Program
{
    private static async Task Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(services =>
            {
                var applicationAssembly = typeof(EntryPoint).Assembly;
                services.AddMediatR(applicationAssembly);
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LogCommandsBehavior<,>));
                services.AddValidatorsFromAssembly(applicationAssembly);

                var cosmosClient = new CosmosClient(EnvironmentConfiguration.GetMandatoryConfiguration("COSMOS_CONNECTION_STRING"));
                var database = cosmosClient.GetDatabase("picshare");
                services.AddSingleton(cosmosClient);
                services.AddSingleton(database);

                services.AddScoped<BlobClient, AzureBlobClient>();
                services.AddScoped<StoreClient, CosmosStoreClient>();
                services.AddScoped<PublisherClient, ServiceBusPublisherClient>();

                var mailingConfiguration = new MailingConfiguration
                {
                    RootUri = EnvironmentConfiguration.GetMandatoryConfiguration("ROOT_URI"),
                    SmtpPort = Convert.ToInt32(EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PORT")),
                    SmtpServer = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_SERVER"),
                    SmtpUser = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_USER"),
                    SmtpPassword = EnvironmentConfiguration.GetMandatoryConfiguration("SMTP_PASSWORD")
                };
                services.AddSingleton(mailingConfiguration);

                services.AddScoped<IEmailWorker, EmailWorker>();
                services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
            })
            .Build();

        await host.RunAsync();
    }
}