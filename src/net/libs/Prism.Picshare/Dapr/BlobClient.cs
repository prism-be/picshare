﻿// -----------------------------------------------------------------------
//  <copyright file = "BlobClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Dapr.Client;
using Microsoft.ApplicationInsights;

namespace Prism.Picshare.Dapr;

public interface IBlobClient
{
    Task Create(string blobName, byte[] data);
    Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default);
}

public class BlobClient : IBlobClient
{
    private readonly DaprClient _daprClient;
    private readonly TelemetryClient _telemetryClient;

    public BlobClient(DaprClient daprClient, TelemetryClient telemetryClient)
    {
        _daprClient = daprClient;
        _telemetryClient = telemetryClient;
    }

    public async Task<List<string>> ListAsync(Guid organisationId, CancellationToken cancellationToken = default)
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

            _telemetryClient.TrackDependency("BINDING", "LIST", organisationId.ToString(), startTime, watch.Elapsed, success);
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

    public async Task Create(string blobName, byte[] data)
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
            });
        }
        finally
        {
            watch.Stop();

            _telemetryClient.TrackDependency("BINDING", "CREATE", blobName, startTime, watch.Elapsed, success);
        }
    }
}