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
        public string CityName { get; set; } = string.Empty;
    }
}
