using System.Text;

namespace WeatherApp.Domain.DomainModels
{
    /// <summary>
    /// Query object to get current weather.
    /// </summary>
    public sealed class CurrentWeatherRequest
    {
        /// <summary>
        /// Gets or sets a city name.
        /// </summary>
        public string CityName { get; set; } = string.Empty;

        public string StateCode { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;        

        public override string ToString()
        {
            StringBuilder currentWeatherQuery = new StringBuilder();

            currentWeatherQuery.Append(CityName);

            // append StateCode only if it is not null or white space.
            if (!string.IsNullOrWhiteSpace(StateCode))
            {
                currentWeatherQuery.Append($", {StateCode}");
            }

            // append CountryCode only if it is not null or white space.
            if (!string.IsNullOrWhiteSpace(CountryCode))
            {
                currentWeatherQuery.Append($", {CountryCode}");
            }

            return currentWeatherQuery.ToString();
        }
    }
}
