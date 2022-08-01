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
using Prism.Picshare.Security;
using Xunit;

namespace Prism.Picshare.Tests.AspNetCore;

public class UserContextAccessorTests
{

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
            new(ClaimsNames.Name, name),
            new Claim(ClaimsNames.UserId, id.ToString()),
            new Claim(ClaimsNames.OrganisationId, organisationId.ToString())
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
    public void HasAccess_Ko()
    {
        // Arrange
        // Arrange
        var id = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var httpContext = new DefaultHttpContext();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new(ClaimsNames.Name, name),
            new Claim(ClaimsNames.UserId, id.ToString()),
            new Claim(ClaimsNames.OrganisationId, organisationId.ToString())
        }, AuthSchemeConstants.PicshareAuthenticationScheme));
        httpContext.User = user;

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var hasAccess = new UserContextAccessor(contextAccessor.Object).HasAccess(Guid.NewGuid());

        // Assert
        hasAccess.Should().BeFalse();
    }

    [Fact]
    public void HasAccess_Ok()
    {
        // Arrange
        // Arrange
        var id = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var name = Guid.NewGuid().ToString();
        var httpContext = new DefaultHttpContext();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new(ClaimsNames.Name, name),
            new Claim(ClaimsNames.UserId, id.ToString()),
            new Claim(ClaimsNames.OrganisationId, organisationId.ToString())
        }, AuthSchemeConstants.PicshareAuthenticationScheme));
        httpContext.User = user;

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var hasAccess = new UserContextAccessor(contextAccessor.Object).HasAccess(organisationId);

        // Assert
        hasAccess.Should().BeTrue();
    }
}