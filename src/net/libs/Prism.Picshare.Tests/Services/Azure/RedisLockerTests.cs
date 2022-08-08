// -----------------------------------------------------------------------
//  <copyright file = "RedisLockerTests.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Exceptions;
using Prism.Picshare.Services.Azure;
using Prism.Picshare.Services.Generic;
using StackExchange.Redis;
using Xunit;

namespace Prism.Picshare.Tests.Services.Azure;

public class RedisLockerTests
{
    [Fact]
    public void Lock_Ok()
    {
        // Arrange
        var logger = new Mock<ILogger<RedisLocker>>();
        var cache = new Mock<IDatabase>();
        var key = Guid.NewGuid().ToString();
        
        // Act
        var locker = new RedisLocker(logger.Object, cache.Object);
        using var locked = locker.GetLock(key);

        // Assert
        cache.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), false, When.Always, CommandFlags.None));
    }
    
    [Fact]
    public void Lock_Failed()
    {
        // Arrange
        var logger = new Mock<ILogger<RedisLocker>>();
        var cache = new Mock<IDatabase>();
        var key = Guid.NewGuid().ToString();
        cache.Setup(x => x.KeyExists(It.IsAny<RedisKey>(), CommandFlags.None))
            .Returns(true);
        
        // Act
        var locker = new RedisLocker(logger.Object, cache.Object)
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };
        Assert.Throws<StoreAccessException>(() => locker.GetLock(key));

        // Assert
        cache.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), false, When.Always, CommandFlags.None), Times.Never);
    }
}