﻿// -----------------------------------------------------------------------
//  <copyright file = "GenerateTokenRequestTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Dapr.Client;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Authentication.Commands;
using Prism.Picshare.Services.Authentication.Configuration;

namespace Prism.Picshare.Services.Authentication.Tests.Commands;

public class GenerateTokenRequestTests
{

    private readonly JwtConfiguration _jwtConfiguration = new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };

    [Fact]
    public async Task Handle_Credentials_Not_Found()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var daprClient = new Mock<DaprClient>();

        // Act
        var handler = new GenerateTokenRequestHandler(logger.Object, daprClient.Object, _jwtConfiguration);
        var result = await handler.Handle(new GenerateTokenRequest(login), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Ok()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Credentials, login, new Credentials
        {
            Id = userId,
            OrganisationId = organisationId,
            Login = login
        });
        daprClient.SetupGetStateAsync(Stores.Users, EntityReference.ComputeKey(organisationId, userId), new User
        {
            Id = userId,
            OrganisationId = organisationId
        });

        // Act
        var handler = new GenerateTokenRequestHandler(logger.Object, daprClient.Object, _jwtConfiguration);
        var result = await handler.Handle(new GenerateTokenRequest(login), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        Debug.Assert(result != null, nameof(result) + " != null");
        result.AccessToken.Should().NotBeNull();
        result.AccessToken.Should().NotBeEmpty();
        result.RefreshToken.Should().NotBeNull();
        result.RefreshToken.Should().NotBeEmpty();
        ValidateToken(_jwtConfiguration.PublicKey, result.AccessToken).Should().BeTrue();
        ValidateToken(_jwtConfiguration.PublicKey, result.RefreshToken).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_User_Not_Found()
    {
        // Arrange
        var login = Guid.NewGuid().ToString();
        var organisationId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var logger = new Mock<ILogger<GenerateTokenRequestHandler>>();
        var daprClient = new Mock<DaprClient>();
        daprClient.SetupGetStateAsync(Stores.Credentials, login, new Credentials
        {
            Id = userId,
            OrganisationId = organisationId,
            Login = login
        });

        // Act
        var handler = new GenerateTokenRequestHandler(logger.Object, daprClient.Object, _jwtConfiguration);
        var result = await handler.Handle(new GenerateTokenRequest(login), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Validate_Empty()
    {
        // Arrange
        var request = new GenerateTokenRequest(String.Empty);

        // Act
        var validator = new GenerateTokenRequestValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_Ok()
    {
        // Arrange
        var request = new GenerateTokenRequest(Guid.NewGuid().ToString());

        // Act
        var validator = new GenerateTokenRequestValidator();
        var result = validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    private bool ValidateToken(string publicKey, string token)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKey);

        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "picshare-authentication",
            ValidAudience = "picshare-front",
            IssuerSigningKey = new RsaSecurityKey(rsa),
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, validationParameters, out _);
        }
        catch
        {
            return false;
        }

        return true;
    }
}