// -----------------------------------------------------------------------
//  <copyright file = "RegisterAccountRequest.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using FluentValidation;
using Isopoh.Cryptography.Argon2;
using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;
using Prism.Picshare.Security;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Authentication;

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
        var credentials = await _storeClient.GetStateNullableAsync<Credentials>(request.Login, cancellationToken);

        if (credentials != default)
        {
            return ResultCodes.ExistingUsername;
        }

        var organisation = new Organisation
        {
            Id = Identifier.Generate(),
            Name = request.Organisation
        };

        credentials = new Credentials
        {
            UserId = Identifier.Generate(),
            OrganisationId = organisation.Id,
            Id = request.Login,
            PasswordHash = Argon2.Hash(request.Password)
        };

        var user = new User
        {
            Id = credentials.UserId,
            OrganisationId = organisation.Id,
            Email = request.Email,
            EmailValidated = false,
            Name = request.Name
        };

        var flow = new Flow
        {
            Id = organisation.Id
        };

        var authorizations = new Authorizations
        {
            Id = user.Id,
            OrganisationId = organisation.Id,
            Pictures = new Dictionary<Guid, string>()
        };

        await _storeClient.CreateStateAsync(organisation.Id.ToString(), organisation, cancellationToken);
        await _storeClient.CreateStateAsync(organisation.Id.ToString(), flow, cancellationToken);
        await _storeClient.CreateStateAsync(credentials.Id, credentials, cancellationToken);
        await _storeClient.CreateStateAsync(user.Id, user, cancellationToken);
        await _storeClient.CreateStateAsync(authorizations.Id, authorizations, cancellationToken);
        await _publisherClient.PublishEventAsync(Topics.User.Register, user, cancellationToken);

        return ResultCodes.Ok;
    }
}