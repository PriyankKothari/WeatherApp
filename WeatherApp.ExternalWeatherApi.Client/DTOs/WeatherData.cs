using Newtonsoft.Json;

namespace WeatherApp.ExternalWeatherApi.Client.DTOs
{
    /// <summary>
    /// Weather Data.
    /// </summary>
    public sealed class WeatherData
    {
        /// <summary>
        /// Gets or sets City name.
        /// </summary>
        [JsonProperty("name")]
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Weather.
        /// </summary>
        [JsonProperty("weather")]
        public ICollection<Weather> WeatherList { get; set; } = new HashSet<Weather>();
    }

    /// <summary>
    /// Weather.
    /// </summary>
    public sealed class Weather
    {
        /// <summary>
        /// Gets or sets description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
    }
}
