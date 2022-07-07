// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class Authentication
{
    private readonly JwtConfiguration _jwtConfiguration = new()
    {
        PrivateKey =
            "MIICXAIBAAKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQABAoGAJdEcGV2o9kzoC+frRC81k3yw1/Y32kZY+JmNZnmatU8UJHNaol7lHnA+PQutc+7JzXEVCP8A+AKC1D59pHs6gql24dCD1Ca9FfS5qTd4SIawUYqcL0OPSVkJPhL+/wVJVOLQG7ALiOhmyNZADSKCj/fKb3bQZH57Sj5osPqGZ4ECQQDwXyBhYwKXZTP4uYTpWX5or/NGF9t1vNdo5tzkspvOFmW81bbGoVg8wzshVmT9cLKlVFHMls1pDWqhCboGHevhAkEAx8x+MGb4haCwQVMnLmthCQmnB7n5uADv2KoVA0zXc7k/reRVgW7aSj4bKNnBrHqdhG8ELDCc02leSESe1J59DQJAevP9yTLvGWgADKNA9Gf9vCj8ZIdBj9kXyqYEqcse3W0hf1VGWBYh33rx3RynLeieyOj3qpIc4jalq1ghWo2loQJBALtMo5tCXIYAjlqe1iM4/H1ZdCDVIhlxn2awgwRV+7/7kIu2esXconxo3lMcV+gWBiZJYFMAu3Og2obK9U6CyN0CQDyfKjEQ4mveAMeYlnDTJmc2CaOVQ2je8SJd5dCwUZbF3nZPgWeuJ/R8jzQy+qxN3ICxRa+D1+ATNm0rmfnb25k=",
        PublicKey =
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7mfR1xgcrRJ+ceZlWpbbXy9dllF9FUXgQU4SuJfby6nXskt2tNX4AOF0dqm/1E1pwy79RhetS/Cj6slnasfwhCI9cwDYVSM1SYsYxr5WXR+KmPUQGC2Upcxt1pB5qj9ow5qq/zOIJ3xwuD+q6VVlAqvF1vKClX/DINToa1I/XbQIDAQAB"
    };

    [Fact]
    public void AddPicshareAuthentication_Ok()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddPicshareAuthentication();

        // Assert
        services.Should().NotBeEmpty();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Failed_Expired()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + GenerateFakeBearer(DateTime.Now.AddSeconds(1)));

        Thread.Sleep(2000);

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Failed_Header_Not_Bearer()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        context.Request.Headers.Add(HeaderNames.Authorization, GenerateFakeBearer());

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Failed_Invalid_Bearer()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + GenerateFakeBearer().Substring(10));

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Failed_No_Public_Key()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + GenerateFakeBearer());

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            new JwtConfiguration());
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Failed_NoHeader()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Ok()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";

        context.Request.Headers.Add(HeaderNames.Authorization, "Bearer " + GenerateFakeBearer());

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Ok_Dapr()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/dapr/authenticate/user";

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_Ok_Events()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());

        var context = new DefaultHttpContext();
        context.Request.Path = "/events/authenticate/user";

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    private string GenerateFakeBearer(DateTime? expires = null)
    {
        expires ??= DateTime.Now.AddMinutes(1);

        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Security.GenerateIdentifier().ToString())
        };

        var privateKey = Convert.FromBase64String(_jwtConfiguration.PrivateKey);

        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(privateKey, out _);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        var now = DateTime.Now;

        var jwt = new JwtSecurityToken(
            audience: JwtConfiguration.Audience,
            issuer: JwtConfiguration.Issuer,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}