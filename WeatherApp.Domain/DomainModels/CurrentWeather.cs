namespace WeatherApp.Domain.DomainModels
{
    /// <summary>
    /// Represents a weather entity.
    /// </summary>
    public sealed class CurrentWeather
    {
        /// <summary>
        /// Gets or sets a City Name.
        /// </summary>
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets weather condition description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
