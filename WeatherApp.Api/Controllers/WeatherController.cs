using Microsoft.AspNetCore.Mvc;
using WeatherApp.Api.Models;
using WeatherApp.Application.UseCases.Weather;
using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class WeatherController : ControllerBase
    {
        private readonly ICurrentWeatherHandler _currentWeatherHandler;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(ICurrentWeatherHandler currentWeatherHandler, ILogger<WeatherController> logger)
        {
            _currentWeatherHandler = currentWeatherHandler ?? throw new ArgumentNullException(nameof(currentWeatherHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet]
        [Route("current")]
        public async Task<IActionResult> Index([FromBody] WeatherRequestModel weatherRequestModel)
        {
            try
            {
                CurrentWeatherRequest request = new(weatherRequestModel.CityName, weatherRequestModel.StateCode, weatherRequestModel.CountryCode);
                return Ok(await _currentWeatherHandler.HandleAsync(request, CancellationToken.None).ConfigureAwait(false));
            }
            catch (Exception exception)
            {
                _logger.LogError("Something went wrong", exception);
                return Problem(exception.Message, exception.StackTrace);
            }
        }
    }
}