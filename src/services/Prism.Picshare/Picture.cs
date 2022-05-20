// -----------------------------------------------------------------------
//  <copyright file="Picture.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare;

public class Picture
{
    public Picture()
    {
        this.Id = Guid.NewGuid();
        this.Source = PictureSource.Unknown;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("source")]
    public PictureSource Source { get; set; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }
}