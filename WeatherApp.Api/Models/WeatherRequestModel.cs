using Microsoft.AspNetCore.Mvc;

namespace WeatherApp.Api.Models
{
    /// <summary>
    /// Weather Request Model.
    /// </summary>
    public class WeatherRequestModel
    {
        /// <summary>
        /// Gets or sets City name.
        /// </summary>
        [FromQuery(Name = "city")]
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Country name.
        /// </summary>
        [FromQuery(Name = "country")]
        public string? CountryName { get; set; }
    }
}
