// -----------------------------------------------------------------------
//  <copyright file = "ClientsExtensions.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Moq;
using Prism.Picshare.Dapr;

namespace Prism.Picshare.UnitTests;

public static class TestsExtensions
{
    public static void SetupGetStateAsync<T>(this Mock<StoreClient> mock, string store, string key, T data) where T : class
    {
        mock.Setup(x => x.GetStateNullableAsync<T>(store, key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);
    }

    public static void VerifyPublishEvent<TExpected>(this Mock<PublisherClient> mock, string expectedTopic)
    {
        mock.Verify(x => x.PublishEventAsync(expectedTopic, It.IsAny<TExpected>(), default), Times.Once);
    }

    public static void VerifyPublishEvent<TExpected>(this Mock<PublisherClient> mock, string expectedTopic, Times times)
    {
        mock.Verify(x => x.PublishEventAsync(expectedTopic, It.IsAny<TExpected>(), default), times);
    }

    public static void VerifySaveState<TExpected>(this Mock<StoreClient> mock, string expectedStore)
    {
        mock.VerifySaveState<TExpected>(expectedStore, Times.Once());
    }

    public static void VerifySaveState<TExpected>(this Mock<StoreClient> mock, string expectedStore, Times times)
    {
        mock.Verify(x => x.SaveStateAsync(expectedStore, It.IsAny<string>(), It.IsAny<TExpected>(), default),
            times);
    }
}