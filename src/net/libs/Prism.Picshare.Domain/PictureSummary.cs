﻿// -----------------------------------------------------------------------
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

    [JsonPropertyName("ready")]
    public bool Ready { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }
}