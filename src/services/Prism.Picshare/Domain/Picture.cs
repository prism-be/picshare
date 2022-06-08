// -----------------------------------------------------------------------
//  <copyright file="Picture.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Picture
{
    public const string Store = "picture-store";

    public Picture()
    {
        Id = Guid.NewGuid();
        Source = PictureSource.Unknown;
        CreationDate = DateTime.UtcNow;
    }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("creationDate")]
    public DateTime CreationDate { get; set; }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }

    [JsonPropertyName("source")]
    public PictureSource Source { get; set; }
}