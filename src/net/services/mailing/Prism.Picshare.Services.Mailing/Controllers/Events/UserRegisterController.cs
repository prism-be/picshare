// -----------------------------------------------------------------------
//  <copyright file = "UserRegisterController.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Dapr;
using Prism.Picshare.Services.Mailing.Commands;

namespace Prism.Picshare.Services.Mailing.Controllers.Events;

public class UserRegisterController: Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisterController> _logger;

    public UserRegisterController(IMediator mediator, ILogger<UserRegisterController> logger)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [Topic(Publishers.PubSub, Topics.User.Register)]
    [HttpPost("events/" + Topics.User.Register)]
    public async Task<IActionResult> Handle([FromBody] User user)
    {
        _logger.LogInformation("Process the event user/register for user - {Id} ({OrganisationId})", user.Id, user.OrganisationId);

        await _mediator.Send(new RegisterConfirmation(user));
        
        return Ok();
    }
}