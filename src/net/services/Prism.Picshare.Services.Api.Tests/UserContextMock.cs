// -----------------------------------------------------------------------
//  <copyright file = "UserContextMock.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.Picshare.AspNetCore.Authentication;

namespace Prism.Picshare.Services.Api.Tests;

public class UserContextMock
{
    public static Mock<IUserContextAccessor> Generate(Guid? organisationId = null, Guid? userId = null, bool hasAccess = true)
    {
        var mock = new Mock<IUserContextAccessor>();
        mock.Setup(x => x.IsAuthenticated).Returns(true);
        mock.Setup(x => x.OrganisationId).Returns(organisationId ?? Guid.NewGuid());
        mock.Setup(x => x.Id).Returns(userId ?? Guid.NewGuid());
        mock.Setup(x => x.HasAccess(It.IsAny<Guid>())).Returns(hasAccess);
        return mock;
    }
}