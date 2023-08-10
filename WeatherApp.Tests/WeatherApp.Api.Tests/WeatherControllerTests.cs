using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<ICurrentWeatherHandler> _currentWeatherHandler;
        private readonly Mock<ILogger<WeatherController>> _logger;

        public WeatherControllerTests()
        {
            _currentWeatherHandler = new Mock<ICurrentWeatherHandler>();
            _logger = new Mock<ILogger<WeatherController>>();
        }

        [TestMethod]
        public void WeatherController_ThrowNullException_When_HandlerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new WeatherController(It.IsAny<ICurrentWeatherHandler>(), _logger.Object));
        }

        [TestMethod]
        public void WeatherController_ThrowNullException_When_LoggerIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => new WeatherController(_currentWeatherHandler.Object, It.IsAny<ILogger<WeatherController>>()));
        }

        [TestMethod]
        public async Task WeatherController_ReturnsBadRequest_When_City_IsNull()
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.City = It.IsAny<string>();

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);
            controller.ModelState.AddModelError("city", "The City field is required.");

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, ((BadRequestObjectResult)result).StatusCode);

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Never);

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "The City field is required." && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsOk_When_City_IsNotNull()
        {
            // Arrange
            //string logInformationMessage = string.Format("City: {0}, Country Code: {1}, Weather Description: {2}");

            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.City = "Mumbai";

            _currentWeatherHandler
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { Data = It.IsAny<CurrentWeather>(), StatusCode = HttpStatusCode.OK});

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, ((OkObjectResult)result).StatusCode);

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            string logInformationMessage = string.Format(
                "City: {0}, Country Code: {1}, Weather Description: {2}",
                (((OkObjectResult)result).Value as CurrentWeather)?.City,
                (((OkObjectResult)result).Value as CurrentWeather)?.CountryCode,
                (((OkObjectResult)result).Value as CurrentWeather)?.Description
            );

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == logInformationMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsOk_When_Country_IsNull()
        {
            // Arrange
            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.City = "Mumbai";
            weatherRequestModel.Object.Country = It.IsAny<string>();

            _currentWeatherHandler
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.OK });

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.AreEqual((int)HttpStatusCode.OK, ((OkObjectResult)result).StatusCode);

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            string logInformationMessage = string.Format(
                "City: {0}, Country Code: {1}, Weather Description: {2}",
                (((OkObjectResult)result).Value as CurrentWeather)?.City,
                (((OkObjectResult)result).Value as CurrentWeather)?.CountryCode,
                (((OkObjectResult)result).Value as CurrentWeather)?.Description
            );

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == logInformationMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsNotFound_When_CityName_IsInvalid()
        {
            // Arrange
            const string cityNameNotFoundError = "City Not Found";

            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.City = "Mum";
            weatherRequestModel.Object.Country = It.IsAny<string>();

            _currentWeatherHandler
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.NotFound, Errors = new List<string> { cityNameNotFoundError } });

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

            NotFoundObjectResult actual = (NotFoundObjectResult)result;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actual.StatusCode);
            Assert.AreEqual(cityNameNotFoundError, (actual.Value as List<string>)?[0]);

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == cityNameNotFoundError && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public async Task WeatherController_ReturnsUnauthorized_When_ExternalWeatherApiKey_IsInvalid()
        {
            // Arrange
            const string inValidApiKeyErrorMessage = "Invalid API key";

            Mock<WeatherRequestModel> weatherRequestModel = new Mock<WeatherRequestModel>();
            weatherRequestModel.Object.City = "Auckland";
            weatherRequestModel.Object.Country = "NZ";

            _currentWeatherHandler
                .Setup(
                    currentWeatherHandler => currentWeatherHandler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    () => new HttpDataResponse<CurrentWeather> { StatusCode = HttpStatusCode.Unauthorized, Errors = new List<string> { inValidApiKeyErrorMessage } });

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);

            // Act
            IActionResult result = await controller.Index(weatherRequestModel.Object, It.IsAny<CancellationToken>()).ConfigureAwait(false);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));

            UnauthorizedObjectResult? actual = result as UnauthorizedObjectResult;
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, actual?.StatusCode);

            string? errorMessage = (actual?.Value as List<string>)?[0];
            Assert.IsTrue(errorMessage?.Contains(inValidApiKeyErrorMessage));

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == inValidApiKeyErrorMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
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
            weatherRequestModel.Object.City = city;
            weatherRequestModel.Object.Country = country;

            _currentWeatherHandler
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

            WeatherController controller = new WeatherController(_currentWeatherHandler.Object, _logger.Object);

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

            _currentWeatherHandler
                .Verify(handler => handler.HandleAsync(It.IsAny<CurrentWeatherRequest>(), It.IsAny<CancellationToken>()), Times.Once);

            string logInformationMessage = string.Format(
                "City: {0}, Country Code: {1}, Weather Description: {2}",
                currentWeather?.City,
                currentWeather?.CountryCode,
                currentWeather?.Description
            );

            _logger.Verify(
                logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == logInformationMessage && @type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
