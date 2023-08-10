using System.ComponentModel.DataAnnotations;

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
        public string? Country { get; set; }
    }
}
