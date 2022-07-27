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
        using var reader = new StreamReader(responseData.Body);
        var json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }
}