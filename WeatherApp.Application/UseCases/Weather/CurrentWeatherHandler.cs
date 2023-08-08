using Microsoft.Extensions.Logging;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Application.UseCases.Weather
{
    /// <summary>
    /// Represents a current weather request handler.
    /// </summary>
    public sealed class CurrentWeatherHandler : ICurrentWeatherHandler
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<ICurrentWeatherHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentWeatherHandler" /> class.
        /// </summary>
        /// <param name="weatherService">A <see cref="IWeatherService" />.</param>
        public CurrentWeatherHandler(IWeatherService weatherService, ILogger<ICurrentWeatherHandler> logger)
        {
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles request to get Current Weather.
        /// </summary>
        /// <param name="request">A request such as City name.</param>
        /// <returns>A <see cref="CurrentWeather" />.</returns>
        public async Task<CurrentWeather> HandleAsync(CurrentWeatherRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(request));

            CurrentWeather? currentWeather = null;

            try
            {
                currentWeather = await _weatherService.GetCurrentWeather(request.ToString(), cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError("An error occured while getting current weather", exception);
            }

            return currentWeather;
        }
    }
}
