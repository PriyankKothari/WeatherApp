using System.ComponentModel.DataAnnotations;
using WeatherApp.Api.Attributes;

namespace WeatherApp.Api.Models
{
    /// <summary>
    /// Weather Request Model.
    /// </summary>
    public class WeatherRequestModel
    {
        /// <summary>
        /// Gets or sets City.
        /// </summary>
        [Required]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Country.
        /// </summary>
        [Required]
        [CountryCode]
        public string? Country { get; set; }
    }
}
