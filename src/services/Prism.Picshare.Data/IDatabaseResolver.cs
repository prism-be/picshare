// -----------------------------------------------------------------------
//  <copyright file="IDatabaseResolver.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data;

public interface IDatabaseResolver
{
    public IDatabase GetDatabase(string organisation, string databaseType);
}