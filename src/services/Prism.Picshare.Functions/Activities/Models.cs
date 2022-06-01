// -----------------------------------------------------------------------
//  <copyright file="Models.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;

namespace Prism.Picshare.Activities;

public record PictureReference(Guid OrganisationId, Guid PictureId, PictureSource Source);

public record PhotoBoothPictureTaken(Guid OrganisationId, Guid PictureId, Guid AlbumId) : PictureReference(OrganisationId, PictureId, PictureSource.Photobooth);