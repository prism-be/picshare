// -----------------------------------------------------------------------
//  <copyright file = "RelaunchUpload.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using Dapr.Client;
using MediatR;
using Prism.Picshare.Dapr;
using Prism.Picshare.Domain;
using Prism.Picshare.Events;

namespace Prism.Picshare.Services.Pictures.Commands.Admin;

public record RelaunchUpload(Guid OrganisationId): IRequest;

public class RelaunchUploadHandler : IRequestHandler<RelaunchUpload>
{
    private readonly DaprClient _daprClient;

    public RelaunchUploadHandler(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public async Task<Unit> Handle(RelaunchUpload request, CancellationToken cancellationToken)
    {
        var bindingRequest = new BindingRequest(Stores.Data, "list");
        bindingRequest.Metadata.Add("prefix", request.OrganisationId + "/");
        bindingRequest.Metadata.Add("fileName", request.OrganisationId + "/");

        var response = await _daprClient.InvokeBindingAsync(bindingRequest, cancellationToken);

        var data = JsonDocument.Parse(Encoding.Default.GetString(response.Data.ToArray()));

        foreach (var element in data.RootElement.EnumerateArray())
        {
            var path = element.GetString();

            if (path == null)
            {
                continue;
            }

            path = path.Replace("\\", "/");

            if (path.EndsWith("/source"))
            {
                var pathSplitted = path.Split('/');
                var organisationId = Guid.Parse(pathSplitted[^3]);
                var pictureId = Guid.Parse(pathSplitted[^2]);
                
                await _daprClient.PublishEventAsync(Publishers.PubSub, Topics.Pictures.Uploaded, new EntityReference
                {
                    OrganisationId = organisationId,
                    Id = pictureId
                }, cancellationToken);
            }
        }
        
        return Unit.Value;
    }
}