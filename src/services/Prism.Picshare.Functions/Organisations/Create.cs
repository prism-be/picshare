// -----------------------------------------------------------------------
//  <copyright file="Create.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Data;

namespace Prism.Picshare.Organisations;

public class Create
{
    private readonly CosmosClient _cosmosClient;

    public Create(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    [FunctionName(nameof(Organisations) + nameof(Create))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var organisation = JsonSerializer.Deserialize<Organisation>(requestBody);

        var db = await _cosmosClient.CreateDatabaseIfNotExistsAsync(nameof(Databases.General));
        
        var organisationContainer = new ContainerProperties
        {
            Id = Databases.General.Organisations,
            PartitionKeyPath = "/id",
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = false,
                IndexingMode = IndexingMode.None
            }
        };
        
        organisationContainer.IndexingPolicy.ExcludedPaths.Clear();
        organisationContainer.IndexingPolicy.IncludedPaths.Clear();
        
        var organisations = await db.Database.CreateContainerIfNotExistsAsync(organisationContainer);

        var response = await organisations.Container.CreateItemAsync(organisation);

        return new StatusCodeResult((int)response.StatusCode);
    }
}