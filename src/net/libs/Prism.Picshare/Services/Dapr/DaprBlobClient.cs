// -----------------------------------------------------------------------
//  <copyright file = "DaprBlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Dapr.Client;
using Microsoft.ApplicationInsights;
using Prism.Picshare.Dapr;

namespace Prism.Picshare.Services.Dapr;

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