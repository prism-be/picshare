// -----------------------------------------------------------------------
//  <copyright file = "EntityReference.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class EntityReference
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }
}