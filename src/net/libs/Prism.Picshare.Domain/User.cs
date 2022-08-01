// -----------------------------------------------------------------------
//  <copyright file = "User.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class User : EntityReference
{
    public User()
    {
        Culture = "en";
        Email = string.Empty;
        Name = string.Empty;
    }

    [JsonPropertyName("emailValidated")]
    public bool EmailValidated { get; set; }

    [JsonPropertyName("culture")]
    public string Culture { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}