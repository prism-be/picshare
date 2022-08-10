// -----------------------------------------------------------------------
//  <copyright file = "FileSystemBlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services.Generic;

public class FileSystemBlobClient : BlobClient
{
    private string BaseDirectory => EnvironmentConfiguration.GetMandatoryConfiguration("LOCAL_BLOB_DIRECTORY");

    public override Task CreateAsync(string blobName, byte[] data, CancellationToken cancellationToken = default)
    {
        var file = Path.Combine(BaseDirectory, blobName);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllBytes(file, data);
        return Task.CompletedTask;
    }

    public override Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(BaseDirectory, organisationId.ToString());
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
            .Select(x => x.Replace(BaseDirectory, string.Empty).Replace("\\", "/"))
            .ToList();
        return Task.FromResult(files);
    }

    public override Task<byte[]> ReadAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var file = Path.Combine(BaseDirectory, blobName);
        return Task.FromResult(File.ReadAllBytes(file));
    }
}