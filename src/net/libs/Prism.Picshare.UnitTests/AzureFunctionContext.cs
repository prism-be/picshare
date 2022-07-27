// -----------------------------------------------------------------------
//  <copyright file = "AzureFunctionContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Moq;

namespace Prism.Picshare.UnitTests;

public static class AzureFunctionContext
{
    public static (Mock<HttpRequestData>, Mock<FunctionContext>) Generate<T>(T body)
    {
        var json = JsonSerializer.Serialize(body);
        var byteArray = Encoding.Default.GetBytes(json);
        var bodyStream = new MemoryStream(byteArray);
        var context = new Mock<FunctionContext>();
        var requestData = new Mock<HttpRequestData>(context.Object);
        requestData.Setup(r => r.Body).Returns(bodyStream);
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