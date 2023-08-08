namespace WeatherApp.Domain.DomainModels
{
    /// <summary>
    /// Query object to get current weather.
    /// </summary>
    public sealed class CurrentWeatherRequest
    {
        /// <summary>
        /// Gets or sets a city.
        /// </summary>
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a country.
        /// </summary>
        public string? CountryName { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(CountryName) ? $"{CityName}, {CountryName}" : CityName;
        }
    }
}
