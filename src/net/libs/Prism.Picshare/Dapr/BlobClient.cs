// -----------------------------------------------------------------------
//  <copyright file = "BlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Dapr.Client;
using Microsoft.ApplicationInsights;

namespace Prism.Picshare.Dapr;

public abstract class BlobClient
{
    public abstract Task CreateAsync(string blobName, byte[] data, CancellationToken cancellationToken = default);
    public abstract Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default);
    public abstract Task<byte[]> ReadAsync(string blobName, CancellationToken cancellationToken = default);
}

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

        return Task.FromResult(container.GetBlobs(prefix: organisationId.ToString())
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

public class DaprBlobClient : BlobClient
{
    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public DaprBlobClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public override async Task CreateAsync(string blobName, byte[] data, CancellationToken cancellationToken = default)
    {
        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            var dataBase64 = Convert.ToBase64String(data);
            await _daprClient.InvokeBindingAsync(Stores.Data, "create", dataBase64, new Dictionary<string, string>
            {
                {
                    "blobName", blobName
                },
                {
                    "fileName", blobName
                }
            }, cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("BINDING", Stores.Data, "CREATE " + blobName, startTime, watch.Elapsed, success);
        }
    }

    public override async Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default)
    {
        var items = new List<string>();

        var bindingRequest = new BindingRequest(Stores.Data, "list");
        bindingRequest.Metadata.Add("prefix", organisationId + "/");
        bindingRequest.Metadata.Add("fileName", organisationId + "/");

        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        BindingResponse? response;

        try
        {
            response = await _daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);
            success = true;
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("BINDING", Stores.Data, "LIST " + organisationId, startTime, watch.Elapsed, success);
        }

        var data = JsonDocument.Parse(Encoding.Default.GetString(response.Data.ToArray()));

        foreach (var element in data.RootElement.EnumerateArray())
        {
            var path = element.GetString();

            if (path == null)
            {
                continue;
            }

            path = path.Replace("\\", "/");

            items.Add(path);
        }

        return items;
    }

    public override async Task<byte[]> ReadAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var bindingRequest = new BindingRequest(Stores.Data, "get");
        bindingRequest.Metadata.Add("blobName", blobName);
        bindingRequest.Metadata.Add("fileName", blobName);

        var startTime = DateTime.UtcNow;
        var watch = Stopwatch.StartNew();
        var success = false;

        try
        {
            var response = await _daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);
            success = true;
            return response.Data.ToArray();
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("BINDING", Stores.Data, "GET " + blobName, startTime, watch.Elapsed, success);
        }
    }
}