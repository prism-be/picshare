// -----------------------------------------------------------------------
//  <copyright file = "AuthenticationMiddleware.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Prism.Picshare.AzureServices.Extensions;
using Prism.Picshare.Security;

namespace Prism.Picshare.AzureServices.Middlewares;

public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(ILogger<AuthenticationMiddleware> logger, JwtConfiguration jwtConfiguration)
    {
        _logger = logger;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var targetMethod = context.GetTargetFunctionMethod();
        var attributes = GetCustomAttributesOnClassAndMethod<AuthorizeAttribute>(targetMethod);

        if (attributes.Count != 1)
        {
            context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
            return;
        }

        var authorization = attributes.Single();

        if (authorization.AllowAnonymous != true)
        {
            var claims = TryGetUserFromBearer(context);

            if (claims == null)
            {
                context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }

            context.Items.Add("PicshareClaims", claims);
        }

        await next(context);
    }

    private static List<T> GetCustomAttributesOnClassAndMethod<T>(MethodInfo targetMethod)
        where T : Attribute
    {
        var methodAttributes = targetMethod.GetCustomAttributes<T>();
        var classAttributes = targetMethod.DeclaringType?.GetCustomAttributes<T>();

        if (classAttributes == null)
        {
            return new List<T>();
        }

        return methodAttributes.Concat(classAttributes).ToList();
    }

    private ClaimsPrincipal? TryGetUserFromBearer(FunctionContext context)
    {
        if (!context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj))
        {
            return null;
        }

        if (headersObj is not string headersStr)
        {
            return null;
        }

        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersStr);

        if (headers == null)
        {
            return null;
        }

        var normalizedKeyHeaders = headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);

        if (!normalizedKeyHeaders.TryGetValue("authorization", out var authHeaderValue))
        {
            return null;
        }

        if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = authHeaderValue.Substring("Bearer ".Length).Trim();
        return TokenGenerator.ValidateToken(_jwtConfiguration.PublicKey, token, _logger, false);
    }
}