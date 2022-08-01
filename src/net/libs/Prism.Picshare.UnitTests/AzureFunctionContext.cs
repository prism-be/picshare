// -----------------------------------------------------------------------
//  <copyright file = "AzureFunctionContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using Moq;
using Prism.Picshare.Security;

namespace Prism.Picshare.UnitTests;

public static class AzureFunctionContext
{
    public static (Mock<HttpRequestData>, Mock<FunctionContext>) Generate(Guid? organisationId = null, Guid? userId = null)
    {
        return Generate<object>(null, organisationId, userId);
    }

    public static (Mock<HttpRequestData>, Mock<FunctionContext>) Generate<T>(T? body, Guid? organisationId = null, Guid? userId = null)
    {
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IOptions<WorkerOptions>)))
            .Returns(Options.Create(new WorkerOptions
            {
                Serializer = new JsonObjectSerializer()
            }));

        var context = new Mock<FunctionContext>();
        context.Setup(x => x.InstanceServices).Returns(serviceProvider.Object);

        var requestData = new Mock<HttpRequestData>(context.Object);

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            var byteArray = Encoding.Default.GetBytes(json);
            var bodyStream = new MemoryStream(byteArray);
            requestData.Setup(r => r.Body).Returns(bodyStream);
        }
        else
        {
            requestData.Setup(r => r.Body).Returns(new MemoryStream());
        }

        var unixTimeSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Identifier.Generate().ToString()),
            new(ClaimsNames.OrganisationId, (organisationId ?? Guid.NewGuid()).ToString()),
            new(ClaimsNames.UserId, (userId ?? Guid.NewGuid()).ToString())
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        context.Setup(x => x.Items).Returns(new Dictionary<object, object>
        {
            {
                "PicshareClaims", principal
            }
        });

        requestData.Setup(r => r.CreateResponse()).Returns(() =>
        {
            var response = new Mock<HttpResponseData>(context.Object);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            response.SetupProperty(r => r.StatusCode);
            response.SetupProperty(r => r.Body, new MemoryStream());
            return response.Object;
        });

        return (requestData, context);
    }
}