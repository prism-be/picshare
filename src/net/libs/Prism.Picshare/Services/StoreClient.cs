// -----------------------------------------------------------------------
//  <copyright file = "StoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Domain;
using Prism.Picshare.Exceptions;

namespace Prism.Picshare.Services;

public abstract class StoreClient
{
    protected static readonly Dictionary<Type, string> StoresMatching = new()
    {
        {
            typeof(Authorizations), Stores.Authorizations
        },
        {
            typeof(Album), Stores.Albums
        },
        {
            typeof(Credentials), Stores.Credentials
        },
        {
            typeof(Flow), Stores.Flow
        },
        {
            typeof(MailAction<>), Stores.MailActions
        },
        {
            typeof(Organisation), Stores.Organisations
        },
        {
            typeof(Picture), Stores.Pictures
        },
        {
            typeof(User), Stores.Users
        }
    };

    public async Task CreateStateAsync<T>(string id, T data, CancellationToken cancellationToken = default)
        where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            var existing = await GetStateNullableAsync<T>(store, string.Empty, id, cancellationToken);

            if (existing != null)
            {
                throw new StoreAccessException("Cannot create an item with a key that already exists", id);
            }

            await SaveStateAsync(store, string.Empty, id, data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task CreateStateAsync<T>(Guid id, T data, CancellationToken cancellationToken = default)
        where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            var organisationId = string.Empty;

            if (data is EntityReference entityReference)
            {
                organisationId = entityReference.OrganisationId.ToString();
            }

            var existing = await GetStateNullableAsync<T>(store, organisationId, id.ToString(), cancellationToken);

            if (existing != null)
            {
                throw new StoreAccessException("Cannot create an item with a key that already exists", $"{organisationId}-{id}");
            }

            await SaveStateAsync(store, organisationId, id.ToString(), data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task<T> GetStateAsync<T>(Guid organisationId, Guid id, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(organisationId, id, cancellationToken);

        if (result == null)
        {
            throw new StoreAccessException("Cannot find entity", $"{organisationId}-{id}");
        }

        return result;
    }

    public async Task<T> GetStateAsync<T>(string id, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(id, cancellationToken);

        if (result == null)
        {
            throw new StoreAccessException("Cannot find entity", id);
        }

        return result;
    }

    public abstract Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default) where T : class;

    public async Task<T?> GetStateNullableAsync<T>(Guid organisationId, Guid id, CancellationToken cancellationToken = default) where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateNullableAsync<T>(store, organisationId.ToString(), id.ToString(), cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task<T?> GetStateNullableAsync<T>(string store, Guid organisationId, Guid id, CancellationToken cancellationToken = default) where T : class
    {
        var organisation = organisationId == Guid.Empty ? String.Empty : organisationId.ToString();
        return await GetStateNullableAsync<T>(store, organisation, id.ToString(), cancellationToken);
    }

    public async Task<T?> GetStateNullableAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateNullableAsync<T>(store, string.Empty, id, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task MutateStateAsync<T>(Guid organisationId, Guid id, Action<T> mutation, CancellationToken cancellationToken = default)
        where T : EntityId
    {
        await MutateStateAsync(organisationId.ToString(), id.ToString(), mutation, cancellationToken);
    }

    public async Task MutateStateAsync<T>(string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
        where T : EntityId
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            await MutateStateAsync(store, organisationId, id, mutation, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public abstract Task MutateStateAsync<T>(string store, string organisationId, string id, Action<T> mutation, CancellationToken cancellationToken = default)
        where T : EntityId;

    public abstract Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default);

    public async Task SaveStateAsync<T>(string id, T data, CancellationToken cancellationToken = default)
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            await SaveStateAsync(store, string.Empty, id, data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task SaveStateAsync<T>(T data, CancellationToken cancellationToken = default)
    {
        if (data is EntityReference entityReference)
        {
            await SaveStateAsync(entityReference.Id, data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot automagically find the Id int type {typeof(T).FullName}");
    }

    public async Task SaveStateAsync<T>(Guid id, T data, CancellationToken cancellationToken = default)
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            var organisationId = string.Empty;

            if (data is EntityReference entityReference)
            {
                organisationId = entityReference.OrganisationId.ToString();
            }

            await SaveStateAsync(store, organisationId, id.ToString(), data, cancellationToken);
            return;
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }
}