// -----------------------------------------------------------------------
//  <copyright file="DatabaseStructure.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Data.CosmosDB;

public static class DatabaseStructure
{
    public static class Pictures
    {
        public const string Database = "picshare";
        public const string Container = "pictures";
    }

    public static class Albums
    {
        public const string Database = "albums";
        public const string Container = "albums";
    }
}