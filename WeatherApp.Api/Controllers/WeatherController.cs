using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherController" /> class.
        /// </summary>
        /// <param name="currentWeatherHandler"><see cref="CurrentWeatherHandler" />.</param>
        public WeatherController(ICurrentWeatherHandler currentWeatherHandler)
        {
            _currentWeatherHandler = currentWeatherHandler ?? throw new ArgumentNullException(nameof(currentWeatherHandler));
        }

        /// <summary>
        /// Gets current weather.
        /// </summary>
        /// <param name="weatherRequestModel"><see cref="WeatherRequestModel" />.</param>
        /// <returns>An <see cref="IActionResult" /> containing current weather data.</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> Index([FromBody] WeatherRequestModel weatherRequestModel)
        {
            CurrentWeather currentWeather;

            try
            {
                CurrentWeatherRequest request = new()
                {
                    CityName = weatherRequestModel.CityName,
                    StateCode = weatherRequestModel.StateCode,
                    CountryCode = weatherRequestModel.CountryCode
                };
                
                currentWeather = await _currentWeatherHandler.HandleAsync(request, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                return Problem(exception.Message, exception.StackTrace);
            }

            return Ok(currentWeather);
        }
    }
}