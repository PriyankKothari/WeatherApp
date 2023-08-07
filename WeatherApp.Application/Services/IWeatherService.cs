using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Application.Services
{
    /// <summary>
    /// Represents a service for weather entity.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Gets current weather data.
        /// </summary>
        /// <param name="location">City name, State code (optional) and Country code (optional without state code, required with state code) separated by comma.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="CurrentWeather" />.</returns>
        Task<CurrentWeather> GetCurrentWeather(string location, CancellationToken cancellationToken);
    }
}
