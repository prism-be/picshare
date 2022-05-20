// -----------------------------------------------------------------------
//  <copyright file="FakeItemResponse.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeItemResponse<T> : ItemResponse<T> where T : new()
{
    private readonly T? _data;

    public FakeItemResponse(HttpStatusCode statusCode, T? data = default)
    {
        this.StatusCode = statusCode;
        this._data = data;
    }

    public override T Resource => this._data ?? new T();

    public override HttpStatusCode StatusCode { get; }
}