using WeatherApp.Application.Abstrations;
using WeatherApp.Domain.DomainModels;

namespace WeatherApp.Application.UseCases.Weather
{
    /// <summary>
    /// Represents an interface for handling current weather request.
    /// </summary>
    public interface ICurrentWeatherHandler : IRequestHandler<CurrentWeatherRequest, CurrentWeather>
    {

    }
}