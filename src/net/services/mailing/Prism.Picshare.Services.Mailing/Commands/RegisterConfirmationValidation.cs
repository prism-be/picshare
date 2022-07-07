// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmationValidation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Services.Mailing.Model;
using Stores = Prism.Picshare.Services.Mailing.Model.Stores;

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
    private readonly DaprClient _daprClient;
    private readonly ILogger<RegisterConfirmationValidationHandler> _logger;

    public RegisterConfirmationValidationHandler(ILogger<RegisterConfirmationValidationHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<ResultCodes> Handle(RegisterConfirmationValidation request, CancellationToken cancellationToken)
    {
        var state = await _daprClient.GetStateAsync<MailAction<User>>(Stores.MailActions, request.Key, cancellationToken: cancellationToken);

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

        var taskPublish = _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Email.Validated, state.Data, cancellationToken);
        var taskSave = _daprClient.SaveStateAsync(Stores.MailActions, state.Key, state, cancellationToken: cancellationToken);

        Task.WaitAll(new[]
        {
            taskPublish,
            taskSave
        }, cancellationToken);

        return ResultCodes.Ok;
    }
}