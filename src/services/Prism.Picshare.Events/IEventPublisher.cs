// -----------------------------------------------------------------------
//  <copyright file="IEventPublisher.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace Prism.Picshare.Events;

public interface IEventPublisher
{
    void Publish<T>(string topic, T message);
}