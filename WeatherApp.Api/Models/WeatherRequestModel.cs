using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    IEnumerable<RegionInfo> regions =
        //        CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));

        //    if (!regions.Any(region => region.EnglishName.Equals(Country, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        yield return new ValidationResult($"{Country} is an invalid country name. Please provide a valid country name.", new[] { nameof(Country) });
        //    }
        //}
    }
}
