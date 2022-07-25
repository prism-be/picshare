// -----------------------------------------------------------------------
//  <copyright file = "RefreshTokenRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Commands;
using Prism.Picshare.UnitTests;

namespace Prism.Picshare.Services.Authentication.Tests.Commands;

public class RefreshTokenRequestTests
{
    private readonly JwtConfiguration _jwtConfiguration = new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };

    [Fact]
    public async Task RefreshTokenRequestHandler_Invalid_Token()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            OrganisationId = organisationId
        };

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Users, EntityReference.ComputeKey(organisationId, userId), user);

        var refreshToken = TokenGenerator.GenerateAccessToken(_jwtConfiguration.PrivateKey, user);

        // Act
        var handler = new RefreshTokenRequestHandler(_jwtConfiguration, logger.Object, storeClient.Object);
        var result = await handler.Handle(new RefreshTokenRequest(refreshToken), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Ok()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            OrganisationId = organisationId
        };

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var storeClient = new Mock<StoreClient>();
        storeClient.SetupGetStateAsync(Stores.Users, EntityReference.ComputeKey(organisationId, userId), user);

        var refreshToken = TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user);

        // Act
        var handler = new RefreshTokenRequestHandler(_jwtConfiguration, logger.Object, storeClient.Object);
        var result = await handler.Handle(new RefreshTokenRequest(refreshToken), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        Debug.Assert(result != null, nameof(result) + " != null");
        result.AccessToken.Should().NotBeNull();
        result.AccessToken.Should().NotBeEmpty();
        result.RefreshToken.Should().NotBeNull();
        result.RefreshToken.Should().NotBeEmpty();
        ValidateToken(_jwtConfiguration.PublicKey, result.AccessToken, false).Should().BeTrue();
        ValidateToken(_jwtConfiguration.PublicKey, result.AccessToken, true).Should().BeFalse();
        ValidateToken(_jwtConfiguration.PublicKey, result.RefreshToken, true).Should().BeTrue();
        ValidateToken(_jwtConfiguration.PublicKey, result.RefreshToken, false).Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Unknown_User()
    {
        // Arrange
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            OrganisationId = organisationId
        };

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var storeClient = new Mock<StoreClient>();

        var refreshToken = TokenGenerator.GenerateRefreshToken(_jwtConfiguration.PrivateKey, user);

        // Act
        var handler = new RefreshTokenRequestHandler(_jwtConfiguration, logger.Object, storeClient.Object);
        var result = await handler.Handle(new RefreshTokenRequest(refreshToken), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    private static bool ValidateToken(string publicKey, string token, bool isRefreshToken)
    {
        return TokenGenerator.ValidateToken(publicKey, token, Mock.Of<ILogger>(), isRefreshToken) != null;
    }
}