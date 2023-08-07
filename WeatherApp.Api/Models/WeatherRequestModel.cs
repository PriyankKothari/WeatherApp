namespace WeatherApp.Api.Models
{
    public class WeatherRequestModel
    {
        /// <summary>
        /// Gets or sets City name.
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Gets or sets State code.
        /// </summary>
        public string StateCode { get; set; }

        /// <summary>
        /// Gets or sets Country code.
        /// </summary>
        public string CountryCode { get; set; }
    }
}
