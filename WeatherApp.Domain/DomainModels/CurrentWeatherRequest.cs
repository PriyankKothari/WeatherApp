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
        public string CityName { get; set; }

        public string StateCode { get; set; }

        public string CountryCode { get; set; }

        public CurrentWeatherRequest(string cityName, string stateCode, string countryCode)
        {
            CityName = cityName;
            StateCode = stateCode;
            CountryCode = countryCode;
        }

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
