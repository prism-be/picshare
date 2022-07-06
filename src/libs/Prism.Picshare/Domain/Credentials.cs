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
        Id = Security.GenerateIdentifier();
        Login = string.Empty;
        PasswordHash = string.Empty;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; }
}