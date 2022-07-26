// -----------------------------------------------------------------------
//  <copyright file = "Credentials.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Credentials
{
    public Credentials()
    {
        UserId = Guid.NewGuid();
        Id = string.Empty;
        PasswordHash = string.Empty;
    }

    [JsonPropertyName("organisationId")]
    public Guid OrganisationId { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; }
}