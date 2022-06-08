﻿// -----------------------------------------------------------------------
//  <copyright file="Organisation.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace Prism.Picshare.Domain;

public class Organisation
{
    public const string Store = "organisation-store";

    public Organisation()
    {
        Id = Guid.NewGuid();
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}