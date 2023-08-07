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
        public string CityName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets weather condition description.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets minimum temperature.
        /// </summary>
        public double MinimumTemperature { get;}

        /// <summary>
        /// Gets or sets maximum temperature.
        /// </summary>
        public double MaximumTemperature { get;}
    }
}
