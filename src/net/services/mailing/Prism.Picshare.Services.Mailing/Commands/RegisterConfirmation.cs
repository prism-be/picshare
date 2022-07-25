// -----------------------------------------------------------------------
//  <copyright file = "RegisterConfirmation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Mailing.Workers;

namespace Prism.Picshare.Services.Mailing.Commands;

public record RegisterConfirmation(User RegisteringUser) : IRequest;

public class RegisterConfirmationHandler : IRequestHandler<RegisterConfirmation>
{
    private readonly IEmailWorker _emailWorker;
    private readonly MailingConfiguration _mailingConfiguration;
    private readonly StoreClient _storeClient;

    public RegisterConfirmationHandler(StoreClient storeClient, IEmailWorker emailWorker, MailingConfiguration mailingConfiguration)
    {
        _storeClient = storeClient;
        _emailWorker = emailWorker;
        _mailingConfiguration = mailingConfiguration;
    }

    public async Task<Unit> Handle(RegisterConfirmation request, CancellationToken cancellationToken)
    {
        var action = new MailAction<User>(Security.GenerateIdentifier(), MailActionType.ConfirmUserRegistration, request.RegisteringUser);
        await _storeClient.SaveStateAsync(Stores.MailActions, action.Key, action, cancellationToken);

        var data = new
        {
            name = request.RegisteringUser.Name,
            validationUrl = $"{_mailingConfiguration.RootUri.Trim('/')}/login/register/validate/{action.Key}"
        };

        await _emailWorker.RenderAndSendAsync("register-confirmation", request.RegisteringUser, data, cancellationToken);

        return Unit.Value;
    }
}