// -----------------------------------------------------------------------
//  <copyright file = "BlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services;

public abstract class BlobClient
{
    public abstract Task CreateAsync(string blobName, byte[] data, CancellationToken cancellationToken = default);
    public abstract Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default);
    public abstract Task<byte[]> ReadAsync(string blobName, CancellationToken cancellationToken = default);
}



