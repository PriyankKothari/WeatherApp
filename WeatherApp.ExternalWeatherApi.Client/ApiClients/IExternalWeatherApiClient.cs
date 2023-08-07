using WeatherApp.ExternalWeatherApi.Client.DTOs;

namespace WeatherApp.ExternalWeatherApi.Client.ApiClients
{
    /// <summary>
    /// Represents Api Client for external weather api.
    /// </summary>
    public interface IExternalWeatherApiClient
    {
        Task<WeatherData> GetCurrentWeather(string location, CancellationToken cancellationToken);
    }
}
