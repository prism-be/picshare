// -----------------------------------------------------------------------
//  <copyright file="Picture.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare;

public class Picture
{
    public Guid Id { get; set; }

    public PictureSource Source { get; set; }

    public Guid OrganisationId { get; set; }
}