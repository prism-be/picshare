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
using Prism.Picshare.UnitTests;
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

        // Act
        var handler = new AuthorizeUserHandler(storeClient.Object, JwtConfigurationFake.JwtConfiguration);
        await handler.Handle(new AuthorizeUser(organisationId, userId, pictureId), CancellationToken.None);

        // Assert
        storeClient.VerifyMutateState<Authorizations>(Stores.Authorizations);
    }

    [Fact]
    public async Task Handle_Ok_Existing()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var pictureId = Guid.NewGuid();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Authorizations, organisationId, userId, new Authorizations
            {
                Id = userId,
                OrganisationId = organisationId,
                Pictures = new Dictionary<Guid, string>
                {
                    {
                        pictureId, Guid.NewGuid().ToString()
                    }
                }
            });

        // Act
        var handler = new AuthorizeUserHandler(storeClient.Object, JwtConfigurationFake.JwtConfiguration);
        await handler.Handle(new AuthorizeUser(organisationId, userId, pictureId), CancellationToken.None);

        // Assert
        storeClient.VerifyMutateState<Authorizations>(Stores.Authorizations);
    }
}