// -----------------------------------------------------------------------
//  <copyright file = "RedisLocker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Azure;
using Microsoft.Extensions.Logging;
using Prism.Picshare.Exceptions;
using StackExchange.Redis;

namespace Prism.Picshare.Services.Generic;

public class RedisLocker
{
    private readonly IDatabase _cache;
    private readonly ILogger<RedisLocker> _logger;

    public RedisLocker(ILogger<RedisLocker> logger, IDatabase cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public int MaxRetries { get; set; } = 30;
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

    public RedisLock GetLock(string key)
    {
        var retries = 0;

        while (retries <= MaxRetries)
        {
            if (_cache.KeyExists(key))
            {
                _logger.LogWarning("Ressource is locked : {key}", key);
            }
            else
            {
                try
                {
                    _cache.StringSet(key, "locked", TimeSpan.FromSeconds(5));
                    return new RedisLock(key, _cache);
                }
                catch (RequestFailedException e)
                {
                    _logger.LogWarning(e, "Lock was already took in the meantime : {key}", key);
                }
            }

            retries++;
            Thread.Sleep(Interval);
        }

        _logger.LogCritical("Lock cannot be took for ressource : {key}", key);
        throw new StoreAccessException("Cannot get lock on key", key);
    }
}

public class RedisLock : IDisposable
{
    private readonly IDatabase _cache;
    private readonly string _key;

    public RedisLock(string key, IDatabase cache)
    {
        _key = key;
        _cache = cache;
    }


    public void Release()
    {
        _cache.KeyDelete(_key);
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}