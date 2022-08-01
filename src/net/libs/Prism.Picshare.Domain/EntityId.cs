// -----------------------------------------------------------------------
//  <copyright file = "SingleId.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class EntityId
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }
}