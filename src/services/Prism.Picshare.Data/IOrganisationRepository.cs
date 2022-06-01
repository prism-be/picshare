// -----------------------------------------------------------------------
//  <copyright file="IOrganisationRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Domain;

namespace Prism.Picshare.Data;

public interface IOrganisationRepository
{
    Task<int> CreateOrganisationAsync(Organisation organisation);
    Task<Organisation?> GetOrganisationAsync(Guid id);
}