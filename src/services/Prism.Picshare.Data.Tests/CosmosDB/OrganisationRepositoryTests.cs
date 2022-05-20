// -----------------------------------------------------------------------
//  <copyright file="OrganisationRepositoryTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Moq;
using Prism.Picshare.Data.CosmosDB;
using Prism.Picshare.Data.Tests.Fakes;
using Xunit;

namespace Prism.Picshare.Data.Tests.CosmosDB;

public class OrganisationRepositoryTests
{
    [Fact]
    public async Task CreateOrganisation_Ok()
    {
        var organisation = new Organisation
        {
            Id = Guid.NewGuid(), Name = "Unit Tests"
        };

        var containerMock = new Mock<Container>();
        containerMock.Setup(x => x.CreateItemAsync(It.IsAny<Organisation>(), null, null, default)).ReturnsAsync(new FakeItemResponse<Organisation>(HttpStatusCode.Created));

        var organisationRepository = new OrganisationRepository(new FakeCosmosClient(containerMock.Object));

        var result = await organisationRepository.CreateOrganisationAsync(organisation);

        Assert.Equal(201, result);
    }

    [Fact]
    public async Task GetOrganisation_Ok()
    {
        var organisation = new Organisation
        {
            Id = Guid.NewGuid(), Name = "Unit Tests"
        };

        var containerMock = new Mock<Container>();
        containerMock.Setup(x => x.ReadItemAsync<Organisation>(organisation.Id.ToString(), It.IsAny<PartitionKey>(), null, default))
            .ReturnsAsync(new FakeItemResponse<Organisation>(HttpStatusCode.OK, organisation));

        var organisationRepository = new OrganisationRepository(new FakeCosmosClient(containerMock.Object));

        var result = await organisationRepository.GetOrganisationAsync(organisation.Id);

        Assert.Equal(organisation, result);
    }
}