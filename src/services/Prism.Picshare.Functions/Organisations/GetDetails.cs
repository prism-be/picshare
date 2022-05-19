// -----------------------------------------------------------------------
//  <copyright file="GetDetails.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Data;

namespace Prism.Picshare.Organisations;

public class GetDetails
{
    private readonly IOrganisationRepository _organisationRepository;

    public GetDetails(IOrganisationRepository organisationRepository)
    {
        _organisationRepository = organisationRepository;
    }

    [FunctionName(nameof(Organisations) + nameof(GetDetails))]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "organisation")]
        HttpRequest req,
        ILogger log)
    {
        if (!req.Query.TryGetValue("id", out var idQuery))
        {
            return new BadRequestResult();
        }

        if (!Guid.TryParse(idQuery, out var id))
        {
            return new BadRequestResult();
        }

        var organisation = await _organisationRepository.GetOrganisationAsync(id);

        if (organisation == null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(organisation);
    }
}