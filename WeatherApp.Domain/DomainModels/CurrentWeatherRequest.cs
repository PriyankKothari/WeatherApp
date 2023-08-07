using System.Text;

namespace WeatherApp.Domain.DomainModels
{
    /// <summary>
    /// Query object to get current weather.
    /// </summary>
    public sealed class CurrentWeatherRequest
    {
        /// <summary>
        /// Gets or sets a city name.
        /// </summary>
        public string CityName { get; set; } = string.Empty;
    }
}
