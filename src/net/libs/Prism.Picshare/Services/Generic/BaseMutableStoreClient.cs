// -----------------------------------------------------------------------
//  <copyright file = "BaseMutableStoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services.Generic;

public abstract class BaseMutableStoreClient : StoreClient
{
    private readonly RedisLocker _locker;

    protected BaseMutableStoreClient(RedisLocker locker)
    {
        _locker = locker;
    }

    public override async Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
    {
        using var locked = _locker.GetLock(id);

        var item = await GetStateNullableAsync<T>(store, organisationId, id, cancellationToken);

        if (item == null)
        {
            throw new StoreAccessException("Cannot mutate inexisting item", $"{store}-{organisationId}-{id}");
        }

        mutation(item);
        await SaveStateAsync(store, organisationId, id, item, cancellationToken);

        locked.Release();
    }
}