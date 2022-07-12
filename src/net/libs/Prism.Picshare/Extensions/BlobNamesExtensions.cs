// -----------------------------------------------------------------------
//  <copyright file = "BlobNamesExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Extensions;

public static class BlobNamesExtensions
{
    public static string GetSourcePath(Guid organisationId, Guid pictureId)
    {
        return $"{organisationId}/{pictureId}/source.jpg";
    }
    
    public static string GetSourcePath(Guid organisationId, Guid pictureId, int width, int height)
    {
        return $"{organisationId}/{pictureId}/{width}-{height}";
    }
}