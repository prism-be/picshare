// -----------------------------------------------------------------------
//  <copyright file="User.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events.Model;

public record UserAuthenticated(Guid Id, ReturnCodes ReturnCode);