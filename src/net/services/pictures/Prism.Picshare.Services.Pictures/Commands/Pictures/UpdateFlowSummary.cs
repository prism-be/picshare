// -----------------------------------------------------------------------
//  <copyright file = "UpdateFlowSummary.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Dapr.Client;
using MediatR;
using Prism.Picshare.Domain;

namespace Prism.Picshare.Services.Pictures.Commands.Pictures;

public record UpdateFlowSummary(PictureSummary Summary) : IRequest<Flow>;

public class UpdateFlowSummaryHandler : IRequestHandler<UpdateFlowSummary, Flow>
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<UpdateFlowSummaryHandler> _logger;

    public UpdateFlowSummaryHandler(ILogger<UpdateFlowSummaryHandler> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

    public async Task<Flow> Handle(UpdateFlowSummary request, CancellationToken cancellationToken)
    {
        var flow = await _daprClient.GetStateFlowAsync(request.Summary.OrganisationId, cancellationToken);

        var existingSummary = flow.Pictures.SingleOrDefault(x => x.Id == request.Summary.Id);

        if (existingSummary == null)
        {
            existingSummary = request.Summary;
            flow.Pictures.Add(existingSummary);
        }
        else
        {
            var existingIndex = flow.Pictures.IndexOf(existingSummary);
            flow.Pictures[existingIndex] = request.Summary;
        }

        flow.Pictures = flow.Pictures.OrderByDescending(x => x.Date).ToList();
        await _daprClient.SaveStateAsync(flow, cancellationToken);

        return flow;
    }
}