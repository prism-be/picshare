// -----------------------------------------------------------------------
//  <copyright file = "EmailValidatedRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Authentication;

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
    private readonly ILogger<EmailValidatedRequestHandler> _logger;
    private readonly StoreClient _storeClient;

    public EmailValidatedRequestHandler(ILogger<EmailValidatedRequestHandler> logger, StoreClient storeClient)
    {
        _logger = logger;
        _storeClient = storeClient;
    }

    public async Task<ResultCodes> Handle(EmailValidatedRequest request, CancellationToken cancellationToken)
    {
        var user = await _storeClient.GetStateNullableAsync<User>(request.OrganisationId, request.UserId, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("The user with reference {key} does not exists", request.UserId);

            return ResultCodes.UserNotFound;
        }

        user.EmailValidated = true;
        await _storeClient.SaveStateAsync(user, cancellationToken: cancellationToken);

        return ResultCodes.Ok;
    }
}