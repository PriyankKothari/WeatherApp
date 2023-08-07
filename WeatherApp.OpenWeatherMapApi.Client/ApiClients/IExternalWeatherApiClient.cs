using WeatherApp.OpenWeatherMapApi.Client.DTOs;

namespace WeatherApp.OpenWeatherMapApi.Client.ApiClients
{
    /// <summary>
    /// Represents Api Client for external weather api.
    /// </summary>
    public interface IExternalWeatherApiClient
    {
        Task<WeatherData> GetCurrentWeather(string location, CancellationToken cancellationToken);
    }
}
