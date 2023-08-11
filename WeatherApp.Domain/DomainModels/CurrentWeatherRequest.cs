using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WeatherApp.Domain.DomainModels
{
    /// <summary>
    /// Query object to get current weather.
    /// </summary>
    public sealed class CurrentWeatherRequest
    {
        /// <summary>
        /// Gets or sets a city.
        /// </summary>
        public string CityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a country.
        /// </summary>
        public string? CountryName { get; set; }

        public override string ToString()
        {
            IEnumerable<RegionInfo> regions =
                CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
            var countryCode =
                regions.FirstOrDefault(region => region.EnglishName.Equals(CountryName, StringComparison.OrdinalIgnoreCase));
            return !string.IsNullOrWhiteSpace(CountryName) ? $"{CityName}, {countryCode}" : CityName;
        }
    }
}
