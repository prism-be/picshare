// -----------------------------------------------------------------------
//  <copyright file = "AuthorizeUserTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Prism.Picshare.Commands.Authorization;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;
using Prism.Picshare.UnitTesting;
using Xunit;

namespace Prism.Picshare.Tests.Commands.Authorization;

public class AuthorizeUserTests
{
    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var storeClient = new Mock<StoreClient>();
        var storeClientMutation = new StoreClientMutatorStub(storeClient);
        var authorizations = new Authorizations
        {
            Id = userId,
            OrganisationId = organisationId,
            Pictures = new Dictionary<Guid, string>()
        };
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, authorizations);

        // Act
        var handler = new AuthorizeUserHandler(storeClientMutation, JwtConfigurationFake.JwtConfiguration);
        await handler.Handle(new AuthorizeUser(organisationId, userId, pictureId), CancellationToken.None);

        // Assert
        storeClient.VerifySaveState<Authorizations>(Stores.Authorizations);
    }

    [Fact]
    public async Task Handle_Ok_Existing()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var storeClient = new Mock<StoreClient>();
        var authorizations = new Authorizations
        {
            Id = userId,
            OrganisationId = organisationId,
            Pictures = new Dictionary<Guid, string>
            {
                {
                    pictureId, Guid.NewGuid().ToString()
                }
            }
        };
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, authorizations);
        var storeClientMutation = new StoreClientMutatorStub(storeClient);

        // Act
        var handler = new AuthorizeUserHandler(storeClientMutation, JwtConfigurationFake.JwtConfiguration);
        await handler.Handle(new AuthorizeUser(organisationId, userId, pictureId), CancellationToken.None);

        // Assert
        storeClient.VerifySaveState<Authorizations>(Stores.Authorizations);
    }

    [Fact]
    public async Task Handle_Ok_Fix_Organisation()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var storeClient = new Mock<StoreClient>();
        var authorizations = new Authorizations
        {
            Id = userId,
            OrganisationId = Guid.Empty,
            Pictures = new Dictionary<Guid, string>
            {
                {
                    pictureId, Guid.NewGuid().ToString()
                }
            }
        };
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, authorizations);
        var storeClientMutation = new StoreClientMutatorStub(storeClient);

        // Act
        var handler = new AuthorizeUserHandler(storeClientMutation, JwtConfigurationFake.JwtConfiguration);
        await handler.Handle(new AuthorizeUser(organisationId, userId, pictureId), CancellationToken.None);

        // Assert
        storeClient.VerifySaveState<Authorizations>(Stores.Authorizations, a => a.OrganisationId == organisationId);
    }
}