// -----------------------------------------------------------------------
//  <copyright file="FakeItemReponse.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeItemResponse<T> : ItemResponse<T>
{
    public FakeItemResponse(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public override HttpStatusCode StatusCode { get; }
}