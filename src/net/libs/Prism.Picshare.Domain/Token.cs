// -----------------------------------------------------------------------
//  <copyright file = "Token.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Token
{
    public Token()
    {
        AccessToken = string.Empty;
        RefreshToken = string.Empty;
    }
    
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("expires")]
    public int Expires { get; set; }
}