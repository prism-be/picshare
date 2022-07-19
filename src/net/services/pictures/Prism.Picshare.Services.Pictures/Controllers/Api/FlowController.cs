// -----------------------------------------------------------------------
//  <copyright file = "FlowController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class FlowController : Controller
{
    private readonly IStoreClient _storeClient;
    private readonly IUserContextAccessor _userContextAccessor;

    public FlowController(IUserContextAccessor userContextAccessor, IStoreClient storeClient)
    {
        _userContextAccessor = userContextAccessor;
        _storeClient = storeClient;
    }

    [HttpGet]
    [Route("api/pictures/flow")]
    public async Task<IActionResult> GetFlow()
    {
        var flow = await _storeClient.GetStateFlowAsync(_userContextAccessor.OrganisationId);

        if (flow != null)
        {
            flow.Pictures = flow.Pictures.Where(x => x.Ready).ToList();
        }

        return Ok(flow);
    }
}