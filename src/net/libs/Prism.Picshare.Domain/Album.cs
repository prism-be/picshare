// -----------------------------------------------------------------------
//  <copyright file = "Album.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Album : EntityReference
{
    public Album()
    {
        Pictures = new List<Guid>();
        Name = string.Empty;
    }

    [JsonPropertyName("pictures")]
    public List<Guid> Pictures { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}