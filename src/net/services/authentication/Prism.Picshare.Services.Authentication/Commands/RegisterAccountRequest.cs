// -----------------------------------------------------------------------
//  <copyright file = "RegisterAccountRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Authentication.Commands;

public record RegisterAccountRequest(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("password")]
    string Password,
    [property: JsonPropertyName("organisation")]
    string Organisation) : IRequest<ResultCodes>;

public class RegisterAccountRequestValidator : AbstractValidator<RegisterAccountRequest>
{
    public RegisterAccountRequestValidator()
    {
        RuleFor(x => x.Organisation).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Login).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(Constants.MaxShortStringLength).EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MaximumLength(Constants.MaxShortStringLength);
    }
}

public class RegisterAccountRequestHandler : IRequestHandler<RegisterAccountRequest, ResultCodes>
{
    private readonly PublisherClient _publisherClient;
    private readonly StoreClient _storeClient;

    public RegisterAccountRequestHandler(StoreClient storeClient, PublisherClient publisherClient)
    {
        _storeClient = storeClient;
        _publisherClient = publisherClient;
    }

    public async Task<ResultCodes> Handle(RegisterAccountRequest request, CancellationToken cancellationToken)
    {
        var organisationId = await _storeClient.GetStateNullableAsync<SingleId>(Stores.OrganisationsName, request.Organisation, cancellationToken);

        if (organisationId != null)
        {
            return ResultCodes.ExistingOrganisation;
        }

        var credentials = await _storeClient.GetStateNullableAsync<Credentials>(request.Login, cancellationToken);

        if (credentials != default)
        {
            return ResultCodes.ExistingUsername;
        }

        var organisation = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = request.Organisation
        };

        organisationId = new SingleId
        {
            Id = organisation.Id
        };

        credentials = new Credentials
        {
            Id = Security.GenerateIdentifier(),
            OrganisationId = organisation.Id,
            Login = request.Login,
            PasswordHash = Argon2.Hash(request.Password)
        };

        var user = new User
        {
            Id = credentials.Id,
            OrganisationId = organisation.Id,
            Email = request.Email,
            EmailValidated = false,
            Name = request.Name
        };

        await _storeClient.SaveStateAsync(Stores.OrganisationsName, organisation.Name, organisationId, cancellationToken);
        await _storeClient.SaveStateAsync(organisation.Id.ToString(), organisation, cancellationToken);
        await _storeClient.SaveStateAsync(credentials.Login, credentials, cancellationToken);
        await _storeClient.SaveStateAsync(user.Key, user, cancellationToken);
        await _publisherClient.PublishEventAsync(Topics.User.Register, user, cancellationToken);

        return ResultCodes.Ok;
    }
}