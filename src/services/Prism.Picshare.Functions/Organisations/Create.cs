// -----------------------------------------------------------------------
//  <copyright file="Create.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Data;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Organisations;

public class Create
{
    private readonly IOrganisationRepository _organisationRepository;

    public Create(IOrganisationRepository organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    [FunctionName(nameof(Organisations) + nameof(Create) + nameof(RunAsync))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "organisations/create")]
        HttpRequest req,
        ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var organisation = JsonSerializer.Deserialize<Organisation>(requestBody);

        if (organisation == null || organisation.Id == Guid.Empty)
        {
            return new BadRequestResult();
        }

        var statusCode = await _organisationRepository.CreateOrganisationAsync(organisation);

        return new StatusCodeResult(statusCode);
    }
}