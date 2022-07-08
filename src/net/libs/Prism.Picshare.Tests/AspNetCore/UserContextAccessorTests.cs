// -----------------------------------------------------------------------
//  <copyright file = "UserContextAccessorTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Prism.Picshare.AspNetCore.Authentication;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class UserContextAccessorTests
{
    [Fact]
    public void Authenticated_Ok()
    {
        // Arrange
        var id = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var httpContext = new DefaultHttpContext();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new("Name", name),
            new Claim("Id", id.ToString()),
            new Claim("OrganisationId", organisationId.ToString())
        }, AuthSchemeConstants.PicshareAuthenticationScheme));
        httpContext.User = user;

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userContextAccessor = new UserContextAccessor(contextAccessor.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeTrue();
        userContextAccessor.Name.Should().Be(name);
        userContextAccessor.Id.Should().Be(id);
        userContextAccessor.OrganisationId.Should().Be(organisationId);
    }
    
    [Fact]
    public void Authenticated_Not_Authenticated()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var userContextAccessor = new UserContextAccessor(contextAccessor.Object);

        // Assert
        userContextAccessor.IsAuthenticated.Should().BeFalse();
        userContextAccessor.Name.Should().BeEmpty();
        userContextAccessor.Id.Should().BeEmpty();
        userContextAccessor.OrganisationId.Should().BeEmpty();
    }
}