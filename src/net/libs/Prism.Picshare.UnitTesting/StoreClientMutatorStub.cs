// -----------------------------------------------------------------------
//  <copyright file = "StoreClientMutatorStub.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.Picshare.Exceptions;
using Prism.Picshare.Services;

namespace Prism.Picshare.UnitTesting;

public class StoreClientMutatorStub : StoreClient
{
    private readonly Mock<StoreClient> _storeClientMock;

    public StoreClientMutatorStub(Mock<StoreClient> storeClientMock)
    {
        _storeClientMock = storeClientMock;
    }

    public override async Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class
    {
        return await _storeClientMock.Object.GetStateNullableAsync<T>(store, organisationId, id, cancellationToken);
    }

    public override async Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
    {
        var data = await GetStateNullableAsync<T>(store, organisationId, id, cancellationToken);

        if (data == null)
        {
            throw new StoreAccessException("The GetStateNullableAsync must be setup", id);
        }

        mutation(data);

        await SaveStateAsync(store, organisationId, id, data, cancellationToken);
    }

    public override async Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default)
    {
        await _storeClientMock.Object.SaveStateAsync(store, organisationId, id, data, cancellationToken);
    }
}