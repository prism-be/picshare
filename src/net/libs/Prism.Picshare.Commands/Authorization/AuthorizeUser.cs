﻿// -----------------------------------------------------------------------
//  <copyright file = "AuthorizeUser.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Prism.Picshare.Domain;
using Prism.Picshare.Security;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Authorization;

public record AuthorizeUser(Guid OrganisationId, Guid UserId, Guid PictureId) : IRequest;

public class AuthorizeUserHandler : IRequestHandler<AuthorizeUser>
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly StoreClient _storeClient;

    public AuthorizeUserHandler(StoreClient storeClient, JwtConfiguration jwtConfiguration)
    {
        _storeClient = storeClient;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<Unit> Handle(AuthorizeUser request, CancellationToken cancellationToken)
    {
        var authorizations = await _storeClient.GetStateNullableAsync<Authorizations>(request.OrganisationId, request.UserId, cancellationToken)
                             ?? new Authorizations
                             {
                                 Id = request.UserId,
                                 OrganisationId = request.OrganisationId
                             };

        if (authorizations.Id == Guid.Empty)
        {
            authorizations.OrganisationId = request.OrganisationId;
            authorizations.Id = request.UserId;
        }

        var token = TokenGenerator.GeneratePictureToken(_jwtConfiguration.PrivateKey, request.UserId, request.PictureId);

        authorizations.Pictures.Remove(request.PictureId);
        authorizations.Pictures.Add(request.PictureId, token);

        await _storeClient.SaveStateAsync(authorizations, cancellationToken);

        return Unit.Value;
    }
}