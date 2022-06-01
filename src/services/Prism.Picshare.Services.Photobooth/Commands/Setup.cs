// -----------------------------------------------------------------------
//  <copyright file="Setup.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using Dapr.Client;
using FluentValidation;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Photobooth.Commands;

public record SetupComplete(Guid OrganisationId, Guid SessionId);

public record Setup(string Organisation) : IRequest<SetupComplete>;

public class SetupValidator : AbstractValidator<Setup>
{
    public SetupValidator()
    {
        this.RuleFor(x => x.Organisation).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class SetupHandler : IRequestHandler<Setup, SetupComplete>
{
    private readonly DaprClient _daprClient;

    public SetupHandler(DaprClient daprClient)
    {
        this._daprClient = daprClient;
    }

    public async Task<SetupComplete> Handle(Setup request, CancellationToken cancellationToken)
    {
        var query = JsonSerializer.Serialize(new
        {
            filter = new
            {
                EQ = new
                {
                    name = request.Organisation
                }
            }
        });

        var results = await this._daprClient.QueryStateAsync<Organisation>(Organisation.Store, query, cancellationToken: cancellationToken);
        var organisation = results.Results.FirstOrDefault()?.Data;

        if (organisation == null)
        {
            organisation = new Organisation
            {
                Id = Guid.NewGuid(), Name = request.Organisation
            };
            await this._daprClient.SaveStateAsync(Organisation.Store, organisation.Id.ToString(), organisation, cancellationToken: cancellationToken);
        }

        return new SetupComplete(organisation.Id, Guid.NewGuid());
    }
}