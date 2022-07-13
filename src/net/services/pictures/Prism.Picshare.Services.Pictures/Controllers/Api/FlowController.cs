// -----------------------------------------------------------------------
//  <copyright file = "FlowController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Services.Pictures.Commands.Pictures;

namespace Prism.Picshare.Services.Pictures.Controllers.Api;

public class FlowController : Controller
{
    private readonly DaprClient _daprClient;
    private readonly IUserContextAccessor _userContextAccessor;

    public FlowController(IUserContextAccessor userContextAccessor, DaprClient daprClient)
    {
        _userContextAccessor = userContextAccessor;
        _daprClient = daprClient;
    }

    [HttpGet]
    [Route("api/pictures/flow")]
    public async Task<IActionResult> GetFlow()
    {
        var flow = await _daprClient.GetStateFlowAsync(_userContextAccessor.OrganisationId, new CancellationToken());
        flow.Pictures = flow.Pictures.Where(x => x.Ready).ToList();
        return Ok(flow);
    }
}