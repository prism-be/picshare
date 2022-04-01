// -----------------------------------------------------------------------
//  <copyright file="Authentication.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

//  <copyright file="Authentication.cs" company="TODO">
//  Copyright (c) TODO. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace

namespace Prism.Picshare.Events.Authentication;

public record LoginRequest(string Organisation, string Login, string Password);

public record LoginResponse(ReturnCodes ReturnCode, string? Token);