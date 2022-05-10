// -----------------------------------------------------------------------
//  <copyright file="IOrganisationRepository.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Microsoft.Azure.Cosmos;

namespace Prism.Picshare.Data;

public interface IOrganisationRepository
{
    
    Task<int> CreateOrganisationAsync(Organisation organisation);
}