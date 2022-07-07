// -----------------------------------------------------------------------
//  <copyright file = "EmailValidationRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Authentication.Commands;

public record EmailValidatedRequest(Guid OrganisationId, Guid UserId) : IRequest<ResultCodes>;

public class EmailValidatedRequestValidator : AbstractValidator<EmailValidatedRequest>
{
    public EmailValidatedRequestValidator()
    {
        RuleFor(x => x.OrganisationId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

public class EmailValidatedRequestHandler : IRequestHandler<EmailValidatedRequest, ResultCodes>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<EmailValidatedRequestHandler> _logger;

    public EmailValidatedRequestHandler(ILogger<EmailValidatedRequestHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<ResultCodes> Handle(EmailValidatedRequest request, CancellationToken cancellationToken)
    {
        var key = EntityReference.ComputeKey(request.OrganisationId, request.UserId);
        var user = await _daprClient.GetStateAsync<User>(Stores.Users, key, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("The user with reference {key} does not exists", key);

            return ResultCodes.UserNotFound;
        }

        user.EmailValidated = true;
        await _daprClient.SaveStateAsync(Stores.Users, key, user, cancellationToken: cancellationToken);

        return ResultCodes.Ok;
    }
}