// -----------------------------------------------------------------------
//  <copyright file = "PictureSummary.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class PictureSummary : EntityReference
{
    public PictureSummary()
    {
        Name = string.Empty;
    }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}