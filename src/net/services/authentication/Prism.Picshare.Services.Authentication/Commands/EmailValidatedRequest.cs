﻿// -----------------------------------------------------------------------
//  <copyright file = "EmailValidationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Commands;

public record EmailValidatedRequest(Guid OrganisationId, Guid UserId) : IRequest<ResponseCodes>;

public class EmailValidatedRequestValidator : AbstractValidator<EmailValidatedRequest>
{
    public EmailValidatedRequestValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class EmailValidatedRequestHandler : IRequestHandler<EmailValidatedRequest, ResponseCodes>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<EmailValidatedRequestHandler> _logger;

    public EmailValidatedRequestHandler(ILogger<EmailValidatedRequestHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<ResponseCodes> Handle(EmailValidatedRequest request, CancellationToken cancellationToken)
    {
        var key = EntityReference.ComputeKey(request.OrganisationId, request.UserId);
        var user = await _daprClient.GetStateAsync<User>(Stores.Users, key, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("The user with reference {key} does not exists", key);

            return ResponseCodes.UserNotFound;
        }

        user.EmailValidated = true;
        await _daprClient.SaveStateAsync(Stores.Users, key, user, cancellationToken: cancellationToken);

        return ResponseCodes.Ok;
    }
}