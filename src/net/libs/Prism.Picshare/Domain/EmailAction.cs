// -----------------------------------------------------------------------
//  <copyright file = "EmailAction.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class EmailAction
{
    public EmailAction()
    {
        Key = Guid.NewGuid().ToString();
        Action = "unknown";
    }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("action")]
    public string Action { get; set; }

    [JsonPropertyName("key")]
    public string Key { get; set; }
}