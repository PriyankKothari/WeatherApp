using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WeatherApp.Api.Attributes
{
    public sealed class CountryCodeAttribute : ValidationAttribute
    {
        public CountryCodeAttribute()
        {
            const string defaultErrorMessage = "Country name is invalid.";
            ErrorMessage ??= defaultErrorMessage;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IEnumerable<RegionInfo> regions =
                        CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));

            return
                regions.Any(region => region.EnglishName.Equals(value?.ToString(), StringComparison.OrdinalIgnoreCase))
                ? ValidationResult.Success
                : value is null ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }
}
