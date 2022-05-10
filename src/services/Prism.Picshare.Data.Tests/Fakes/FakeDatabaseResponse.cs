// -----------------------------------------------------------------------
//  <copyright file="FakeDatabaseResponse.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeDatabaseResponse: DatabaseResponse
{
    private readonly Container _container;

    public FakeDatabaseResponse(Container container)
    {
        _container = container;
    }

    public override Database Database => new FakeDatabase(_container);
}