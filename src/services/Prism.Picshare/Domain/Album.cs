// -----------------------------------------------------------------------
//  <copyright file="Album.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Album
{
    public Album(List<Guid> pictures)
    {
        this.Pictures = pictures;
        this.Name = string.Empty;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }

    [JsonPropertyName("pictures")]
    public List<Guid> Pictures { get; set; }
}