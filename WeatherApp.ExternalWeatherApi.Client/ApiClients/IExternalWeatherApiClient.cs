namespace WeatherApp.ExternalWeatherApi.Client.ApiClients
{
    /// <summary>
    /// Represents Api Client for external weather api.
    /// </summary>
    public interface IExternalWeatherApiClient
    {
        Task<HttpResponseMessage> GetCurrentWeather(string location, CancellationToken cancellationToken);
    }
}
