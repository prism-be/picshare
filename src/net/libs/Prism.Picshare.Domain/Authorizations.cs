// -----------------------------------------------------------------------
//  <copyright file = "Authorizations.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Authorizations : EntityReference
{
    public Authorizations()
    {
        Pictures = new Dictionary<Guid, string>();
    }

    [JsonPropertyName("pictures")]
    public Dictionary<Guid, string> Pictures { get; set; }
}