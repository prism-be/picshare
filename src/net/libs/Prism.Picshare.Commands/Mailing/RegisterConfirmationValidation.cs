// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationValidation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Mailing;

public record RegisterConfirmationValidation(Guid Id) : IRequest<ResultCodes>;

public class RegisterConfirmationValidationValidation : AbstractValidator<RegisterConfirmationValidation>
{
    public RegisterConfirmationValidationValidation()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class RegisterConfirmationValidationHandler : IRequestHandler<RegisterConfirmationValidation, ResultCodes>
{
    private readonly ILogger<RegisterConfirmationValidationHandler> _logger;
    private readonly PublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public RegisterConfirmationValidationHandler(ILogger<RegisterConfirmationValidationHandler> logger, StoreClient storeClient, PublisherClient publisherClient)
    {
        _logger = logger;
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<ResultCodes> Handle(RegisterConfirmationValidation request, CancellationToken cancellationToken)
    {
        var state = await _storeClient.GetStateNullableAsync<MailAction<User>>(Stores.MailActions, Guid.Empty, request.Id, cancellationToken);

        if (state == null)
        {
            _logger.LogInformation("The state for key {Id} does not exists", request.Id);
            return ResultCodes.MailActionNotFound;
        }

        if (state.Consumed)
        {
            _logger.LogInformation("The state for key {Id} is already consumed", request.Id);
            return ResultCodes.MailActionAlreadyConsumed;
        }

        state.Consumed = true;
        state.ConfirmationDate = DateTime.UtcNow;

        var taskPublish = _publisherClient.PublishEventAsync(Topics.Email.Validated, state.Data, cancellationToken);
        var taskSave = _storeClient.SaveStateAsync(Stores.MailActions, string.Empty, state.Id.ToString(), state, cancellationToken);

        Task.WaitAll(new[]
        {
            taskPublish,
            taskSave
        }, cancellationToken);

        return ResultCodes.Ok;
    }
}