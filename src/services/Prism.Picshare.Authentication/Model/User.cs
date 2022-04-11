// -----------------------------------------------------------------------
//  <copyright file="User.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Authentication.Model;

public record User(Guid Id, string Login, string PasswordHash, DateTime LastLogin);