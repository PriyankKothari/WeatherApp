using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using WeatherApp.Application.Services;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Domain.DomainModels;
using WeatherApp.Domain.HttpResponseModels;

namespace WeatherApp.Tests.WeatherApp.Application.Tests
{
    [TestClass]
    public class CurrentWeatherHandlerTests
    {
        private readonly Mock<IWeatherService> _weatherService;
        private readonly Mock<ILogger<ICurrentWeatherHandler>> _logger;

        private CurrentWeatherHandler _currentWeatherHandler;

        public CurrentWeatherHandlerTests()
        {
            _weatherService = new Mock<IWeatherService>();
            _logger = new Mock<ILogger<ICurrentWeatherHandler>>();
        }

        [TestMethod]
        public void CurrentWeatherHandler_ThrowArgumentNullException_When_WeatherServiceIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new CurrentWeatherHandler(It.IsAny<IWeatherService>(), _logger.Object));
        }

        [TestMethod]
        public void CurrentWeatherHandler_ThrowArgumentNullException_When_LoggerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new CurrentWeatherHandler(_weatherService.Object, It.IsAny<ILogger<ICurrentWeatherHandler>>()));
        }

        [TestMethod]
        public async Task CurrentWeatherHandler_ThrowArgumentNullException_When_CurrentWeatherRequest_IsNull()
        {
            // Arrange

            // Act

            // Assert
            await
                Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                    await
                        _currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()).ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CurrentWeatherHandler_Returns_HttpDataResponse_When_CurrentWeatherRequest_IsValid()
        {
            // Arrange
            _weatherService
                .Setup(service => service.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.OK });

            _currentWeatherHandler = new CurrentWeatherHandler(_weatherService.Object, _logger.Object);

            CurrentWeatherRequest currentWeatherRequest = new CurrentWeatherRequest { CityName = "London", CountryName = "GB" };

            // Act
            var response = await _currentWeatherHandler.HandleAsync(currentWeatherRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(response, typeof(HttpDataResponse<CurrentWeather>));
        }

        [TestMethod]
        [DataRow("Mumbai", "India", "IN", "Haze")]
        [DataRow("Sydney", "Australia", "AU", "Few Clouds")]
        [DataRow("New York", "United States of America", "US", "Broken Clouds")]
        [DataRow("Wellington", "New Zealand", "NZ", "Broken Clouds")]
        [DataRow("Berlin", "Germany", "DE", "Clear Sky")]
        public async Task CurrentWeatherHandler_Returns_HttpDataResponse_With_StatusCode_And_Data_When_Request_IsValid(string city, string country, string code, string description)
        {
            // Arrange
            _weatherService
                .Setup(service => service.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather>
                    {
                        Data = new CurrentWeather
                        {
                            City = city,
                            CountryCode = code,
                            Description = description
                        },
                        StatusCode = HttpStatusCode.OK
                    });

            _currentWeatherHandler = new CurrentWeatherHandler(_weatherService.Object, _logger.Object);

            CurrentWeatherRequest currentWeatherRequest = new CurrentWeatherRequest { CityName = city, CountryName = country };

            // Act
            var response = await _currentWeatherHandler.HandleAsync(currentWeatherRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);

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
            _weatherService
                .Setup(service => service.GetCurrentWeather(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { Errors = new List<string> { "City not found" }, StatusCode = HttpStatusCode.NotFound });

            _currentWeatherHandler = new CurrentWeatherHandler(_weatherService.Object, _logger.Object);

            CurrentWeatherRequest currentWeatherRequest = new CurrentWeatherRequest { CityName = city, CountryName = country };

            // Act
            var response = await _currentWeatherHandler.HandleAsync(currentWeatherRequest, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            Assert.IsTrue(response.Errors?.Any());
        }
    }
}
