// -----------------------------------------------------------------------
//  <copyright file = "SingleId.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class SingleId
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}