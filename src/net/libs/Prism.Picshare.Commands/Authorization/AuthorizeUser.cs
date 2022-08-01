// -----------------------------------------------------------------------
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
        var token = TokenGenerator.GeneratePictureToken(_jwtConfiguration.PrivateKey, request.OrganisationId, request.UserId, request.PictureId);

        await _storeClient.MutateStateAsync<Authorizations>(request.OrganisationId, request.UserId, authorizations =>
        {
            if (authorizations.Id == Guid.Empty)
            {
                authorizations.OrganisationId = request.OrganisationId;
                authorizations.Id = request.UserId;
            }

            authorizations.Pictures.Remove(request.PictureId);
            authorizations.Pictures.Add(request.PictureId, token);
        }, cancellationToken);

        return Unit.Value;
    }
}