using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using WeatherApp.Api.Controllers;
using WeatherApp.Api.Models;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Domain.DomainModels;
using WeatherApp.Domain.HttpResponseModels;

namespace WeatherApp.Tests.WeatherApp.Api.Tests
{
    [TestClass]
    public class WeatherControllerTests
    {
        private readonly Mock<ICurrentWeatherHandler> _currentWeatherHandlerMock = new Mock<ICurrentWeatherHandler>();        

        [TestMethod]
        public void WeatherController_ThrowNullException_When_HandlerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new WeatherController(It.IsAny<ICurrentWeatherHandler>()));
        }

        [TestMethod]
        public async Task WeatherController_ReturnsBadRequest_When_City_IsNull()
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = It.IsAny<string>();

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);
            controller.ModelState.AddModelError("city", "The CityName field is required.");

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsOk_When_City_IsNotNull()
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = "Mumbai";

            _currentWeatherHandlerMock
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { Data = It.IsAny<CurrentWeather>(), StatusCode = HttpStatusCode.OK});

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsOk_When_Country_IsNull()
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = "Mumbai";
            weatherRequestModel.Object.CountryName = It.IsAny<string>();

            _currentWeatherHandlerMock
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.OK });

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, ((OkObjectResult)result).StatusCode);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsNotFound_When_CityName_IsInvalid()
        {
            // Arrange
            const string cityNameNotFoundError = "City Not Found";

            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = "Mum";
            weatherRequestModel.Object.CountryName = It.IsAny<string>();

            _currentWeatherHandlerMock
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.NotFound, Errors = new List<string> { cityNameNotFoundError } });

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

            NotFoundObjectResult actual = (NotFoundObjectResult)result;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actual.StatusCode);
            Assert.AreEqual(cityNameNotFoundError, (actual.Value as List<string>)?[0]);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsUnauthorized_When_ExternalWeatherApiKey_IsInvalid()
        {
            // Arrange
            const string inValidApiKeyErrorMessage = "Invalid API key";

            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = "Auckland";
            weatherRequestModel.Object.CountryName = "NZ";

            _currentWeatherHandlerMock
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.Unauthorized, Errors = new List<string> { inValidApiKeyErrorMessage } });

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));

            UnauthorizedObjectResult? actual = result as UnauthorizedObjectResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, actual?.StatusCode);

            string? errorMessage = (actual?.Value as List<string>)?[0];
            Assert.IsTrue(errorMessage?.Contains(inValidApiKeyErrorMessage));
        }

        [TestMethod]
        [DataRow("Mumbai", "India", "IN", "Haze")]
        [DataRow("Sydney", "Australia", "AU", "Few Clouds")]
        [DataRow("New York", "United States of America", "US", "Broken Clouds")]
        [DataRow("Wellington", "New Zealand", "NZ", "Broken Clouds")]
        [DataRow("Berlin", "Germany", "DE", "Clear Sky")]
        public async Task WeatherController_Returns_CurrentWeather_When_WeatherRequestModel_IsValid(string city, string country, string code, string desc)
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.CityName = city;
            weatherRequestModel.Object.CountryName = country;

            _currentWeatherHandlerMock
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather>
                    {
                        Data = new CurrentWeather
                        {
                            City = city,
                            CountryCode = code,
                            Description = desc
                        },
                        StatusCode = HttpStatusCode.OK 
                    });

            WeatherController controller = new WeatherController(_currentWeatherHandlerMock.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));

            OkObjectResult? actual = result as OkObjectResult;
            Assert.AreEqual((int)HttpStatusCode.OK, actual?.StatusCode);

            CurrentWeather? currentWeather = actual?.Value as CurrentWeather;
            Assert.AreEqual(city, currentWeather?.City);
            Assert.AreEqual(code, currentWeather?.CountryCode);
            Assert.AreEqual(desc, currentWeather?.Description);
        }
    }
}
