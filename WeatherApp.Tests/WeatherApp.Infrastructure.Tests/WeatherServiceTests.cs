using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Net;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DomainModels;
using WeatherApp.Domain.HttpResponseModels;
using WeatherApp.ExternalWeatherApi.Client.ApiClients;
using WeatherApp.ExternalWeatherApi.Client.DTOs;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Tests.WeatherApp.Infrastructure.Tests
{
    [TestClass]
    public class WeatherServiceTests
    {
        private readonly Mock<IExternalWeatherApiClient> _externalWeatherApiHttpClient;
        private readonly Mock<ILogger<IWeatherService>> _logger;

        private WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _externalWeatherApiHttpClient = new Mock<IExternalWeatherApiClient>();
            _logger = new Mock<ILogger<IWeatherService>>();
        }

        [TestMethod]
        public void WeatherService_ThrowNullException_When_ExternalWeatherApiClientIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new WeatherService(It.IsAny<IExternalWeatherApiClient>(), It.IsAny<ILogger<IWeatherService>>()));
        }

        [TestMethod]
        public void WeatherService_ThrowNullException_When_LoggerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new WeatherService(_externalWeatherApiHttpClient.Object, It.IsAny<ILogger<IWeatherService>>()));
        }

        [TestMethod]
        public async Task WeatherService_ThrowNullException_When_LocationIsNull()
        {
            // Arrange
            _weatherService = new WeatherService(_externalWeatherApiHttpClient.Object, _logger.Object);

            // Act

            // Assert
            await
                Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                    await
                        _weatherService.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task WeatherService_Returns_HttpDataResponse_When_Location_IsValid()
        {
            _externalWeatherApiHttpClient
                .Setup(apiHttpClient => apiHttpClient.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new WeatherData())),
                        StatusCode = HttpStatusCode.OK
                    });

            _weatherService = new WeatherService(_externalWeatherApiHttpClient.Object, _logger.Object);

            // Act
            var response = await _weatherService.GetCurrentWeather("mumbai", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(response, typeof(HttpDataResponse<CurrentWeather>));
        }

        [TestMethod]
        [DataRow("Mumbai", "India", "IN", "Haze")]
        [DataRow("Sydney", "Australia", "AU", "Few Clouds")]
        [DataRow("New York", "United States of America", "US", "Broken Clouds")]
        [DataRow("Wellington", "New Zealand", "NZ", "Broken Clouds")]
        [DataRow("Berlin", "Germany", "DE", "Clear Sky")]
        public async Task WeatherService_Returns_HttpDataResponse_With_StatusCode_And_Data_When_Location_IsValid(string city, string country, string code, string description)
        {
            // Arrange
            var content = new WeatherData
            {
                City = city,
                System = new ExternalWeatherApi.Client.DTOs.System
                {
                        CountryCode = code
                },
                WeatherList = new List<Weather> 
                {
                    new Weather
                    {
                        Description = description
                    }
                }
            };

            _externalWeatherApiHttpClient
                .Setup(apiHttpClient => apiHttpClient.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(content)),
                        StatusCode = HttpStatusCode.OK
                    });

            _weatherService = new WeatherService(_externalWeatherApiHttpClient.Object, _logger.Object);

            // Act
            var response = await _weatherService.GetCurrentWeather($"{city}, {country}", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(response.Data);
        }

        [TestMethod]
        [DataRow("Mum", "India")]
        [DataRow("Syd", "Australia")]
        [DataRow("New Yk", "United States of America")]
        [DataRow("Welli", "New Zealand")]
        [DataRow("Ber", "Germany")]
        public async Task CurrentWeatherHandler_Returns_HttpDataResponse_With_StatusCode_And_ErrorMessage_When_Request_IsInvalid(string city, string country)
        {
            // Arrange
            var content = new WeatherData
            {
                City = city,
                ErrorMessage = "City Not Found"
            };

            _externalWeatherApiHttpClient
                .Setup(apiHttpClient => apiHttpClient.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpResponseMessage
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(content)),
                        StatusCode = HttpStatusCode.NotFound
                    });

            _weatherService = new WeatherService(_externalWeatherApiHttpClient.Object, _logger.Object);

            // Act
            var response = await _weatherService.GetCurrentWeather($"{city}, {country}", It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsTrue(response.Errors?.Any());
        }
    }
}
