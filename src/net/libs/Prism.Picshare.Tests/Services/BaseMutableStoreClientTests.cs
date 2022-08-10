// -----------------------------------------------------------------------
//  <copyright file = "BaseMutableStoreClientTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Domain;
using Prism.Picshare.Services.Generic;
using StackExchange.Redis;
using Xunit;

namespace Prism.Picshare.Tests.Services;

public class BaseMutableStoreClientTests
{
    private sealed class FakeMutableStore : BaseMutableStoreClient
    {
        public FakeMutableStore(RedisLocker locker) : base(locker)
        {
        }

        public override Task<T?> GetStateNullableAsync<T>(string store, string organisationId, string id, CancellationToken cancellationToken = default)
            where T : class

        {
            var result = Activator.CreateInstance<T?>();
            return Task.FromResult(result);
        }

        public override Task SaveStateAsync<T>(string store, string organisationId, string id, T data, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
    
    [Fact]
    public async Task MutateStateAsync_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RedisLocker>>();
        var database = new Mock<IDatabase>();
        
        // Act
        var store = new FakeMutableStore(new RedisLocker(logger.Object, database.Object));
        await store.MutateStateAsync<User>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), x => x.Id = Guid.NewGuid(), CancellationToken.None);

        // Assert
        database.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), false, When.Always, CommandFlags.None));
    }
}