using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Picshare.Insights.Middlewares;
using Xunit;

namespace Prism.Picshare.Tests.Insights;

public class ErrorLoggingMiddlewareTests
{
    [Fact]
    public async Task Invoke_Should_Log_Error_On_Exception()
    {
        // Arrange
        var requestDelegateMock = new Mock<RequestDelegate>();
        var exception = new ApplicationException();
        requestDelegateMock.Setup(x => x.Invoke(It.IsAny<HttpContext>())).Throws(exception);
        var iloggerMock = new Mock<ILogger<ErrorLoggingMiddleware>>();
        var errorLogginMiddleware = new ErrorLoggingMiddleware(requestDelegateMock.Object, iloggerMock.Object);

        // Act
        _ = await Assert.ThrowsAsync<ApplicationException>(async () => await errorLogginMiddleware.InvokeAsync(Mock.Of<HttpContext>()));

        // Assert
        iloggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_Should_No_Log_Wen_NoError()
    {
        // Arrange
        var requestDelegateMock = new Mock<RequestDelegate>();
        var iloggerMock = new Mock<ILogger<ErrorLoggingMiddleware>>();
        var errorLogginMiddleware = new ErrorLoggingMiddleware(requestDelegateMock.Object, iloggerMock.Object);

        // Act
        await errorLogginMiddleware.InvokeAsync(Mock.Of<HttpContext>());

        // Assert
        iloggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Never);
    }
}