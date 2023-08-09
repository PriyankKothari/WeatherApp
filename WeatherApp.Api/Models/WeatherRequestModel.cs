using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Country name.
        /// </summary>
        [FromQuery(Name = "country")]
        public string? CountryName { get; set; }
    }
}
