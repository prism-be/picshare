// -----------------------------------------------------------------------
//  <copyright file="PictureUploaded.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Events;

public record PicureCreated(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("organisationId")]
    Guid OrganisationId,
    [property: JsonPropertyName("albumId")]
    Guid? AlbumId)
{
    public const string Topic = "pictures/created";
}