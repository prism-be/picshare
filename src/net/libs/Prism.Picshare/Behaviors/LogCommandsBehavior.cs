// -----------------------------------------------------------------------
//  <copyright file = "ValidationBehavior.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;

namespace Prism.Picshare.Behaviors;

public sealed class LogCommandsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly ILogger<LogCommandsBehavior<TRequest, TResponse>> _logger;

    public LogCommandsBehavior(ILogger<LogCommandsBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Processing command '{command}' with data : {request}", typeof(TRequest).FullName, request);
        
        return await next();
    }
}