// -----------------------------------------------------------------------
//  <copyright file="RabbitConfiguration.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events.Rabbit;

public class RabbitConfiguration
{
    public string? Exchange { get; set; }

    public Uri? Uri { get; set; }
}