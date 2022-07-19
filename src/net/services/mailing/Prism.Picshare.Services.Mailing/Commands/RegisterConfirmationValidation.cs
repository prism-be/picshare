// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationValidation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using FluentValidation;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Mailing.Commands;

public record RegisterConfirmationValidation(string Key) : IRequest<ResultCodes>;

public class RegisterConfirmationValidationValidation : AbstractValidator<RegisterConfirmationValidation>
{
    public RegisterConfirmationValidationValidation()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class RegisterConfirmationValidationHandler : IRequestHandler<RegisterConfirmationValidation, ResultCodes>
{
    private readonly ILogger<RegisterConfirmationValidationHandler> _logger;
    private readonly IPublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public RegisterConfirmationValidationHandler(ILogger<RegisterConfirmationValidationHandler> logger, StoreClient storeClient, IPublisherClient publisherClient)
    {
        _logger = logger;
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<ResultCodes> Handle(RegisterConfirmationValidation request, CancellationToken cancellationToken)
    {
        var state = await _storeClient.GetStateNullableAsync<MailAction<User>>(Stores.MailActions, request.Key, cancellationToken);

        if (state == null)
        {
            _logger.LogInformation("The state for key {key} does not exists", request.Key);
            return ResultCodes.MailActionNotFound;
        }

        if (state.Consumed)
        {
            _logger.LogInformation("The state for key {key} is already consumed", request.Key);
            return ResultCodes.MailActionAlreadyConsumed;
        }

        state.Consumed = true;
        state.ConfirmationDate = DateTime.UtcNow;

        var taskPublish = _publisherClient.PublishEventAsync(Topics.Email.Validated, state.Data, cancellationToken);
        var taskSave = _storeClient.SaveStateAsync(Stores.MailActions, state.Key, state, cancellationToken);

        Task.WaitAll(new[]
        {
            taskPublish,
            taskSave
        }, cancellationToken);

        return ResultCodes.Ok;
    }
}