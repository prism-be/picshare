// -----------------------------------------------------------------------
//  <copyright file = "UserAuthentication.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Services.Api.Contracts;

public class UserAuthentication
{
    [JsonPropertyName("authenticated")]
    public bool Authenticated { get; set; }

    [JsonPropertyName("organisation")]
    public Guid Organisation { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}