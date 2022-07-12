// -----------------------------------------------------------------------
//  <copyright file = "ExifData.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class ExifData
{
    public ExifData()
    {
        Tag = Type = Value = string.Empty;
    }

    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}