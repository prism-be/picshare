// -----------------------------------------------------------------------
//  <copyright file = "PhotoboothPicture.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class PhotoboothPicture : EntityReference
{
    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }

    [JsonPropertyName("originalFileName")]
    public string? OriginalFileName { get; set; }
}