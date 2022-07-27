// -----------------------------------------------------------------------
//  <copyright file = "HttpRequestDataExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace Prism.Picshare.AzureServices.Extensions;

public static class HttpRequestDataExtensions
{
    public static async Task<HttpResponseData> CreateResponseAsync<T>(this HttpRequestData requestData, HttpStatusCode statusCode, T data)
    {
        var response = requestData.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(data);
        response.StatusCode = statusCode;
        return response;
    }
}