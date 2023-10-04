using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using WeatherApp.Api.Middlewares;

namespace WeatherApp.Tests.WeatherApp.Api.Tests
{
    [TestClass]
    public class ApiKeyAuthenticationMiddlewareTests
    {
        private readonly Mock<ILogger<ApiKeyAuthenticationMiddleware>> _logger;

        public ApiKeyAuthenticationMiddlewareTests()
        {
            _logger = new Mock<ILogger<ApiKeyAuthenticationMiddleware>>();
        }

        [TestMethod]
        public void ApiKeyAuthenticationMiddleware_ThrowArgumentNullException_When_RequestDelegateIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ApiKeyAuthenticationMiddleware(
                    It.IsAny<RequestDelegate>(),
                    new ConfigurationBuilder().Build(),
                    _logger.Object)
                );
        }

        [TestMethod]
        public void ApiKeyAuthenticationMiddleware_ThrowArgumentNullException_When_ConfigurationIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    It.IsAny<IConfiguration>(),
                    _logger.Object)
                );
        }

        [TestMethod]
        public void ApiKeyAuthenticationMiddleware_ThrowArgumentNullException_When_LoggerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    new ConfigurationBuilder().Build(),
                    It.IsAny<ILogger<ApiKeyAuthenticationMiddleware>>())
                );
        }

        [TestMethod]
        public async Task ApiKeyAuthenticationMiddleware_ThrowArgumentNullReferenceException_When_CurrentHttpContext_IsNull()
        {
            // Arrange
            ApiKeyAuthenticationMiddleware middlewareInstance =
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    new ConfigurationBuilder().Build(),
                    _logger.Object
                );

            // Act

            // Assert
            await
                Assert.ThrowsExceptionAsync<ArgumentNullException>(
                    async () =>
                        await middlewareInstance.Invoke(null).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ApiKeyAuthenticationMiddleware_ReturnsUnauthorized_WhenApiKey_IsNull()
        {
            // Arrange
            const string missingApiKeyOutput = "\"MISSING API KEY\"";
            const string missingApiKeyLogMessage = "Authentication failed. API KEY is not provided with the request.";

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            ApiKeyAuthenticationMiddleware middlewareInstance =
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    new ConfigurationBuilder().Build(),
                    _logger.Object
                );

            // Act
            await middlewareInstance.Invoke(httpContext).ConfigureAwait(false);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(missingApiKeyOutput, new StreamReader(httpContext.Response.Body).ReadToEnd());

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Critical),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == missingApiKeyLogMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ApiKeyAuthenticationMiddleware_ReturnsUnauthorized_WhenApiKey_IsInvalid()
        {
            // Arrange
            const string missingApiKeyOutput = "\"INVALID API KEY\"";
            const string missingApiKeyLogMessage = "Authentication failed. Valid API KEY is not provided with the request.";

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("X-API-KEY", "X-API-KEY");
            httpContext.Response.Body = new MemoryStream();

            string appSettings =
                @"{""Authentication"": {""ApiKeys"": {""X-API-KEY"": [ ""Authentication_Api_Key_One"" ]}}}";

            IConfiguration configuration =
                new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)))
                .Build();

            ApiKeyAuthenticationMiddleware middlewareInstance =
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    configuration,
                    _logger.Object
                );

            // Act
            await middlewareInstance.Invoke(httpContext).ConfigureAwait(false);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, httpContext.Response.StatusCode);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(missingApiKeyOutput, new StreamReader(httpContext.Response.Body).ReadToEnd());

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Critical),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == missingApiKeyLogMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task ApiKeyAuthenticationMiddleware_ReturnsOk_WhenApiKey_IsValid()
        {
            // Arrange
            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("X-API-KEY", "ApiKeyOne");
            httpContext.Response.Body = new MemoryStream();

            string appSettings =
                @"{""Authentication"": {""ApiKeys"": {""X-API-KEY"": [ ""ApiKeyOne"" ]}}}";

            IConfiguration configuration =
                new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)))
                .Build();

            ApiKeyAuthenticationMiddleware middlewareInstance =
                new ApiKeyAuthenticationMiddleware(
                    (nextRequestDelegate) => { return Task.CompletedTask; },
                    configuration,
                    _logger.Object
                );

            // Act
            await middlewareInstance.Invoke(httpContext).ConfigureAwait(false);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
        }
    }
}