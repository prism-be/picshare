// -----------------------------------------------------------------------
//  <copyright file = "Authentication.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
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
    public async Task HandleAuthenticateAsync_Ok()
    {
        // Arrange
        var options = new Mock<IOptionsMonitor<PicshareAuthenticationScheme>>();
        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new PicshareAuthenticationScheme());
        
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/authenticate/user";
        
        var authorizationHeader = new StringValues(String.Empty);
        context.Request.Headers.Add(HeaderNames.Authorization, authorizationHeader);

        var handler = new PicshareAuthenticationHandler(options.Object, new NullLoggerFactory(), UrlEncoder.Default, Mock.Of<ISystemClock>(),
            _jwtConfiguration);
        await handler.InitializeAsync(new AuthenticationScheme(AuthSchemeConstants.PicshareAuthenticationScheme, null, typeof(PicshareAuthenticationHandler)), context);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
    }
}