// -----------------------------------------------------------------------
//  <copyright file="FakeContainerResponse.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data.Tests.Fakes;

public class FakeContainerResponse : ContainerResponse
{
    private readonly Container _container;

    public FakeContainerResponse(Container container)
    {
        _container = container;
    }

    public override Container Container => _container;
}