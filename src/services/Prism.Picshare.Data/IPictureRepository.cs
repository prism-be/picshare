// -----------------------------------------------------------------------
//  <copyright file="IPictureRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data;

public interface IPictureRepository
{
    public Task<int> Upsert(Guid organisationId, Picture picture);

    public Task<Picture> Get(Guid organisationId, Guid pictureId);
}