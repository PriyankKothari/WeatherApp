using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using WeatherApp.Application.Services;
using WeatherApp.Domain.DomainModels;
using WeatherApp.Domain.HttpResponseModels;
using WeatherApp.ExternalWeatherApi.Client.ApiClients;
using WeatherApp.ExternalWeatherApi.Client.DTOs;

namespace WeatherApp.Infrastructure.Services
{
    /// <summary>
    /// Implements service to work with weather entity and External Weather Api.
    /// </summary>
    public sealed class WeatherService : IWeatherService
    {
        private readonly IExternalWeatherApiClient _externalWeatherApiHttpClient;
        private readonly ILogger<IWeatherService> _logger;

        /// <summary>
        /// Initializes a new instant of the <see cref="WeatherService" /> class.
        /// </summary>
        /// <param name="externalWeatherApiHttpClient"><see cref="IExternalWeatherApiClient" />.</param>
        /// <param name="logger"><see cref="ILogger" />.</param>
        public WeatherService(IExternalWeatherApiClient externalWeatherApiHttpClient, ILogger<IWeatherService> logger)
        {
            _externalWeatherApiHttpClient = externalWeatherApiHttpClient ?? throw new ArgumentNullException(nameof(externalWeatherApiHttpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets current weather data.
        /// </summary>
        /// <param name="location">City name, State code (optional) and Country code (optional without state code, required with state code) separated by comma.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="HttpDataResponse{CurrentWeather}" />.</returns>
        public async Task<HttpDataResponse<CurrentWeather>> GetCurrentWeather(string location, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(location, nameof(location));

            HttpDataResponse<CurrentWeather>? httpDataResponse = new HttpDataResponse<CurrentWeather>();

            try
            {
                HttpResponseMessage responseMessage =
                    await _externalWeatherApiHttpClient.GetCurrentWeather(location, cancellationToken).ConfigureAwait(false);

                string weatherDataJson = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(weatherDataJson))
                {
                    WeatherData? weatherData = JsonConvert.DeserializeObject<WeatherData>(weatherDataJson);

                    httpDataResponse.Data = new CurrentWeather
                    {
                        City = weatherData?.City ?? string.Empty,
                        CountryCode = weatherData?.System?.CountryCode ?? string.Empty,
                        Description = weatherData?.WeatherList?.FirstOrDefault()?.Description ?? string.Empty,
                    };
                    httpDataResponse.Errors = new List<string> { weatherData?.ErrorMessage ?? string.Empty };
                }
                else
                {
                    httpDataResponse.Errors = new List<string> { responseMessage.ReasonPhrase?.ToString() ?? string.Empty };
                }

                httpDataResponse.StatusCode = responseMessage.StatusCode;
            }
            catch (Exception exception)
            {
                _logger.LogError("Something went wrong while attempting to get current weather using External Weather Api Client", exception);
                httpDataResponse.Errors = new List<string> { exception.Message };
                httpDataResponse.StatusCode = HttpStatusCode.InternalServerError;
            }

            return httpDataResponse;
        }
    }
}