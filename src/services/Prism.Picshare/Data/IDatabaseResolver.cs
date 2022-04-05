// -----------------------------------------------------------------------
//  <copyright file="IDatabaseResolver.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using LiteDB;

namespace Prism.Picshare.Data;

public interface IDatabaseResolver
{
    public ILiteDatabase GetDatabase(string organisation, string databaseType);
}