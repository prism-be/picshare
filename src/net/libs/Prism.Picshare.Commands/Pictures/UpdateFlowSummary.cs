// -----------------------------------------------------------------------
//  <copyright file = "UpdateFlowSummary.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Domain;
using Prism.Picshare.Services;

namespace Prism.Picshare.Commands.Pictures;

public record UpdateFlowSummary(PictureSummary Summary) : IRequest<Flow>;

public class UpdateFlowSummaryHandler : IRequestHandler<UpdateFlowSummary, Flow>
{
    private readonly ILogger<UpdateFlowSummaryHandler> _logger;
    private readonly StoreClient _storeClient;

    public UpdateFlowSummaryHandler(ILogger<UpdateFlowSummaryHandler> logger, StoreClient storeClient)
    {
        _logger = logger;
        _storeClient = storeClient;
    }

    public async Task<Flow> Handle(UpdateFlowSummary request, CancellationToken cancellationToken)
    {
        var flow = await _storeClient.GetStateAsync<Flow>(request.Summary.OrganisationId.ToString(), cancellationToken);

        var existingSummary = flow.Pictures.SingleOrDefault(x => x.Id == request.Summary.Id);

        if (existingSummary == null)
        {
            _logger.LogInformation("The summary {id} does not exist in flow {flow}, adding", request.Summary.Id, request.Summary.OrganisationId);
            existingSummary = request.Summary;
            flow.Pictures.Add(existingSummary);
        }
        else
        {
            var existingIndex = flow.Pictures.IndexOf(existingSummary);
            flow.Pictures[existingIndex] = request.Summary;
        }

        flow.Pictures = flow.Pictures.OrderByDescending(x => x.Date).ToList();
        await _storeClient.SaveStateAsync(request.Summary.OrganisationId.ToString(), flow, cancellationToken);

        return flow;
    }
}