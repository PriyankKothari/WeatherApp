using Microsoft.Extensions.Logging;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DomainModels;
using WeatherApp.OpenWeatherMapApi.Client.ApiClients;
using WeatherApp.OpenWeatherMapApi.Client.DTOs;

namespace WeatherApp.Infrastructure.Services
{
    /// <summary>
    /// Implements service to work with weather entity and External Weather Api.
    /// </summary>
    public sealed class WeatherService : IWeatherService
    {
        private readonly IExternalWeatherApiClient _openWeatherMapHttpClient;
        private readonly ILogger<IWeatherService> _logger;

        /// <summary>
        /// Initializes a new instant of the <see cref="WeatherService" /> class.
        /// </summary>
        /// <param name="openWeatherMapHttpClient"><see cref="IExternalWeatherApiClient" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public WeatherService(IExternalWeatherApiClient openWeatherMapHttpClient, ILogger<IWeatherService> logger)
        {
            _openWeatherMapHttpClient = openWeatherMapHttpClient ?? throw new ArgumentNullException(nameof(openWeatherMapHttpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets current weather data.
        /// </summary>
        /// <param name="location">City name, State code (optional) and Country code (optional without state code, required with state code) separated by comma.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="CurrentWeather" />.</returns>
        public async Task<CurrentWeather> GetCurrentWeather(string location, CancellationToken cancellationToken)
        {
            CurrentWeather currentWeather = null;

            try
            {
                WeatherData currentWeatherResult =
                    await _openWeatherMapHttpClient.GetCurrentWeather(location, cancellationToken).ConfigureAwait(false);

                currentWeather = new CurrentWeather
                {
                    CityName = currentWeatherResult.CityName,
                    Description = currentWeatherResult.WeatherList[0].Description
                };
            }
            catch (Exception exception)
            {
                _logger.LogError($"{nameof(IWeatherService)} cannot get current weather.", exception);
            }

            return currentWeather;            
        }
    }
}
