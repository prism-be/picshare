// -----------------------------------------------------------------------
//  <copyright file="PhotoboothPicture.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class PhotoboothPicture
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }

    [JsonPropertyName("sessionId")]
    public Guid SessionId { get; set; }
}