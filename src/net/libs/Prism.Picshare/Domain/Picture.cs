// -----------------------------------------------------------------------
//  <copyright file = "Picture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Picture : EntityReference
{
    public Picture()
    {
        Id = Guid.NewGuid();
        Source = PictureSource.Unknown;
        CreationDate = DateTime.UtcNow;
        Name = string.Empty;
    }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("creationDate")]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("views")]
    public int Views { get; set; }

    [JsonPropertyName("source")]
    public PictureSource Source { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}