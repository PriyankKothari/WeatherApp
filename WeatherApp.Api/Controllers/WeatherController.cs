using Microsoft.AspNetCore.Mvc;
using System.Net;
using WeatherApp.Api.Models;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Api.Controllers
{
    /// <summary>
    /// Weather Controller.
    /// </summary>
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class WeatherController : ControllerBase
    {
        private readonly ICurrentWeatherHandler _currentWeatherHandler;
        private readonly ILogger<WeatherController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController" /> class.
        /// </summary>
        /// <param name="currentWeatherHandler"><see cref="CurrentWeatherHandler" />.</param>
        /// <param name="logger"><see cref="ILogger{WeatherController}" />.</param>
        public WeatherController(ICurrentWeatherHandler currentWeatherHandler, ILogger<WeatherController> logger)
        {
            _currentWeatherHandler = currentWeatherHandler ?? throw new ArgumentNullException(nameof(currentWeatherHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets current weather.
        /// </summary>
        /// <param name="weatherRequestModel"><see cref="WeatherRequestModel" />.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>An <see cref="IActionResult" /> containing current weather data.</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> Index([FromQuery] WeatherRequestModel weatherRequestModel, CancellationToken cancellationToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning(string.Join("; ", ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                CurrentWeatherRequest request = new()
                {
                    CityName = weatherRequestModel.City,
                    CountryName = weatherRequestModel.Country ?? string.Empty
                };

                var httpDataResponse = await _currentWeatherHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

                if (httpDataResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(string.Join("; ", httpDataResponse.Errors));
                    return NotFound(httpDataResponse.Errors);
                }

                if (httpDataResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning(string.Join("; ", httpDataResponse.Errors));
                    return Unauthorized(httpDataResponse.Errors);
                }

                _logger.LogInformation($"City: {httpDataResponse?.Data?.City}, Country Code: {httpDataResponse?.Data?.CountryCode}, Weather Description: {httpDataResponse?.Data?.Description}");
                return Ok(httpDataResponse?.Data);
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception.Message} - {exception.StackTrace}");
                return Problem(exception.Message, exception.StackTrace);
            }
        }
    }
}