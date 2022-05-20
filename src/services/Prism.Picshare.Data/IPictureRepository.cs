// -----------------------------------------------------------------------
//  <copyright file="IPictureRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Net;

namespace Prism.Picshare.Data;

public interface IPictureRepository
{
    public Task Upsert(Guid organisationId, Picture picture);

    public Task<Picture?> Get(Guid organisationId, Guid pictureId);
}