// -----------------------------------------------------------------------
//  <copyright file = "AzureBlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Azure.Storage.Blobs;

namespace Prism.Picshare.Services.Azure;

public class AzureBlobClient : BlobClient
{
    public override async Task CreateAsync(string blobName, byte[] data, CancellationToken cancellationToken = default)
    {
        var container = new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_BLOB_CONNECTION_STRING"), "picshare");
        var blob = container.GetBlobClient(blobName);
        await blob.UploadAsync(new BinaryData(data), cancellationToken);
    }

    public override Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        var container = new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_BLOB_CONNECTION_STRING"), "picshare");

        return Task.FromResult(container.GetBlobs(prefix: organisationId.ToString(), cancellationToken: cancellationToken)
            .Select(x => x.Name)
            .ToList());
    }

    public override async Task<byte[]> ReadAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var container = new BlobContainerClient(EnvironmentConfiguration.GetMandatoryConfiguration("AZURE_BLOB_CONNECTION_STRING"), "picshare");
        var blob = container.GetBlobClient(blobName);
        var response = await blob.DownloadContentAsync(cancellationToken);
        return response.Value.Content.ToArray();
    }
}