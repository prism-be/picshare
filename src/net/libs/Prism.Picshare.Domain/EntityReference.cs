// -----------------------------------------------------------------------
//  <copyright file = "EntityReference.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class EntityReference : EntityId
{
    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }
}