// -----------------------------------------------------------------------
//  <copyright file = "PictureFlow.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Flow
{
    public Flow()
    {
        Pictures = new List<PictureSummary>();
    }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }

    [JsonPropertyName("pictures")]
    public List<PictureSummary> Pictures { get; set; }
}