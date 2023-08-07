using WeatherApp.Application.Services;
using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Infrastructure.Services
{
    /// <summary>
    /// Weather service implementation to work with External Weather Api.
    /// </summary>
    public sealed class WeatherService : IWeatherService
    {
        /// <summary>
        /// Gets current weather data.
        /// </summary>
        /// <param name="location">City name, State code (optional) and Country code (optional without state code, required with state code) separated by comma.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="CurrentWeather" />.</returns>
        public async Task<CurrentWeather> GetCurrentWeather(string location, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(location));

            throw new NotImplementedException();
        }
    }
}
