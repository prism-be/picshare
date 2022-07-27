// -----------------------------------------------------------------------
//  <copyright file = "StoreClient.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Prism.Picshare.Domain;

namespace Prism.Picshare.Services;

public abstract class StoreClient
{
    protected static readonly Dictionary<Type, string> StoresMatching = new()
    {
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

    protected static readonly HashSet<Type> OrganisationScopedTypes = new()
    {
        typeof(Album),
        typeof(Picture),
        typeof(User)
    };

    public async Task<T> GetStateAsync<T>(string id, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(id, cancellationToken);
        return result ?? new T();
    }

    public async Task<T> GetStateAsync<T>(string store, string key, CancellationToken cancellationToken = default) where T : class, new()
    {
        var result = await GetStateNullableAsync<T>(store, key, cancellationToken);
        return result ?? new T();
    }

    public abstract Task<T?> GetStateNullableAsync<T>(string store, string organisation, string id, CancellationToken cancellationToken = default) where T : class;

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

    public async Task<T?> GetStateNullableAsync<T>(string partition, string id, CancellationToken cancellationToken = default) where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateNullableAsync<T>(store, partition, id, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public async Task<T?> GetStateNullableAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
    {
        if (StoresMatching.TryGetValue(typeof(T), out var store))
        {
            return await GetStateNullableAsync<T>(store, string.Empty, id, cancellationToken);
        }

        throw new NotImplementedException($"Cannot find store for type {typeof(T).FullName}");
    }

    public abstract Task SaveStateAsync<T>(string store, string organisation, string id, T data, CancellationToken cancellationToken = default);

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