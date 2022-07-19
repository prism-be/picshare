// -----------------------------------------------------------------------
//  <copyright file = "FlowController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class FlowController : Controller
{
    private readonly StoreClient _storeClient;
    private readonly IUserContextAccessor _userContextAccessor;

    public FlowController(IUserContextAccessor userContextAccessor, StoreClient storeClient)
    {
        _userContextAccessor = userContextAccessor;
        _storeClient = storeClient;
    }

    [HttpGet]
    [Route("api/pictures/flow")]
    public async Task<IActionResult> GetFlow()
    {
        var flow = await _storeClient.GetStateAsync<Flow>(_userContextAccessor.OrganisationId.ToString());

        flow.Pictures = flow.Pictures.Where(x => x.Ready).ToList();

        return Ok(flow);
    }
}