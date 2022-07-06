// -----------------------------------------------------------------------
//  <copyright file = "Organisation.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Organisation
{
    public Organisation()
    {
        Id = Guid.NewGuid();
        Users = new List<Guid>();
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("users")]
    public List<Guid> Users { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}