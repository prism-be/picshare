// -----------------------------------------------------------------------
//  <copyright file = "UserContext.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Security;

public record UserContext(Guid OrganisationId, Guid Id, bool IsAuthenticated, string Name);