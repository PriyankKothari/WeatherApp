using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using WeatherApp.ExternalWeatherApi.Client.ApiClients;
using WeatherApp.ExternalWeatherApi.Client.Options;

namespace WeatherApp.Tests.WeatherApp.ExternalWeatherApi.Client.Tests
{
    [TestClass]
    public class ExternalWeatherApiClientTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<ILogger<ExternalWeatherApiClient>> _logger;
        private readonly Mock<IOptions<ExternalWeatherApiOptions>> _options;

        private const string apiBaseUrl = "https://api.openweathermap.org/data/2.5";
        private const string apiEndpoint = "weather";
        private const string apiKey = "8b7535b42fe1c551f18028f64e8688f7";

        private ExternalWeatherApiClient _externalWeatherApiClient;

        public ExternalWeatherApiClientTests()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _logger = new Mock<ILogger<ExternalWeatherApiClient>>();
            _options = new Mock<IOptions<ExternalWeatherApiOptions>>();
        }

        [TestMethod]
        public void ExternalWeatherApiClient_ThrowNullException_When_HttpClientFactoryIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalWeatherApiClient(It.IsAny<IHttpClientFactory>(), _logger.Object, _options.Object));
        }

        [TestMethod]
        public void ExternalWeatherApiClient_ThrowNullException_When_LoggerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalWeatherApiClient(_httpClientFactory.Object, It.IsAny<ILogger<ExternalWeatherApiClient>>(), _options.Object));
        }

        [TestMethod]
        public void ExternalWeatherApiClient_ThrowNullException_When_OptionsIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new ExternalWeatherApiClient(_httpClientFactory.Object, _logger.Object, It.IsAny<IOptions<ExternalWeatherApiOptions>>()));
        }

        [TestMethod]
        public async Task ExternalWeatherApiClient_ThrowNullException_When_LocationIsNull()
        {
            // Arrange
            _externalWeatherApiClient = new ExternalWeatherApiClient(_httpClientFactory.Object, _logger.Object, Options.Create(new ExternalWeatherApiOptions()));

            // Act

            // Assert
            await
                Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                    await
                        _externalWeatherApiClient.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ExternalWeatherApiClient_Returns_InternalServerError_WhenRequestUri_IsInvalid()
        {
            // Arrange
            ExternalWeatherApiOptions externalWeatherApiOptions = new ExternalWeatherApiOptions();

            // HttpMessageHandler
            Mock<HttpMessageHandler> httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            // HttpClient
            HttpClient httpClient = new HttpClient(httpMessageHandler.Object);

            // HttpClientFactory
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _externalWeatherApiClient =
                new ExternalWeatherApiClient(_httpClientFactory.Object, _logger.Object, Options.Create(externalWeatherApiOptions));

            // Act
            var response = await _externalWeatherApiClient.GetCurrentWeather("some location", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(HttpResponseMessage));

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            
        }

        [TestMethod]
        public async Task ExternalWeatherApiClient_Returns_Ok_When_Locaton_IsValid()
        {
            // Arrange
            ExternalWeatherApiOptions externalWeatherApiOptions =
                new ExternalWeatherApiOptions { ApiBaseUrl = apiBaseUrl, ApiEndpoint = apiEndpoint, ApiKey = apiKey};

            // HttpMessageHandler
            Mock<HttpMessageHandler> httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            // HttpClient
            HttpClient httpClient = new HttpClient(httpMessageHandler.Object);

            // HttpClientFactory
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _externalWeatherApiClient =
                new ExternalWeatherApiClient(_httpClientFactory.Object, _logger.Object, Options.Create(externalWeatherApiOptions));

            // Act
            var response = await _externalWeatherApiClient.GetCurrentWeather("some location", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(HttpResponseMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);            
        }

        [TestMethod]
        public async Task ExternalWeatherApiClient_Returns_HttpResponseMessage_WithContent_When_Location_IsValid()
        {
            // Arrange
            ExternalWeatherApiOptions externalWeatherApiOptions =
                new ExternalWeatherApiOptions { ApiBaseUrl = apiBaseUrl, ApiEndpoint = apiEndpoint, ApiKey = apiKey };

            // HttpMessageHandler
            Mock<HttpMessageHandler> httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(@"""{""weather"":[{""description"":""haze""}],""sys"":{""country"":""IN""},""name"":""Mumbai""}""")) });

            // HttpClient
            HttpClient httpClient = new HttpClient(httpMessageHandler.Object);

            // HttpClientFactory
            _httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            _externalWeatherApiClient =
                new ExternalWeatherApiClient(_httpClientFactory.Object, _logger.Object, Options.Create(externalWeatherApiOptions));

            // Act
            var response = await _externalWeatherApiClient.GetCurrentWeather("some location", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(HttpResponseMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("OK", response.ReasonPhrase);
            Assert.IsNotNull(response.Content);
        }
    }
}
