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
        Tag = Type = string.Empty;
        Value = new object();
    }

    [JsonPropertyName("tag")]
    public string Tag { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("value")]
    public dynamic Value { get; set; }
}