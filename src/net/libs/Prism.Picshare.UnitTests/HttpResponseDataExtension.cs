// -----------------------------------------------------------------------
//  <copyright file = "HttpResponseDataExtension.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace Prism.Picshare.UnitTests;

public static class HttpResponseDataExtension
{
    public static T? GetBody<T>(this HttpResponseData responseData)
    {
        responseData.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseData.Body);
        var json = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<T>(json);
    }
}