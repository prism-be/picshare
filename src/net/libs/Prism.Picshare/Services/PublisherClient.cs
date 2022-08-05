// -----------------------------------------------------------------------
//  <copyright file = "PublisherClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Services;

public abstract class PublisherClient
{
    public abstract Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default);

    public abstract Task PublishEventsAsync<T>(string topic, IEnumerable<T> data, CancellationToken cancellationToken = default);
}