using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherApp.ExternalWeatherApi.Client.DTOs;
using WeatherApp.ExternalWeatherApi.Client.Options;

namespace WeatherApp.ExternalWeatherApi.Client.ApiClients
{
    /// <summary>
    /// Implements methods communicating with the external weather api.
    /// </summary>
    public sealed class ExternalWeatherApiClient : IExternalWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<ExternalWeatherApiOptions> _options;

        /// <summary>
        /// Initiates a new instance of the <see cref="ExternalWeatherApiClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory"><see cref="IHttpClientFactory" />.</param>
        /// <param name="options"><see cref="IOptions{ExternalWeatherApiOptions}" />.</param>
        public ExternalWeatherApiClient(IHttpClientFactory httpClientFactory, IOptions<ExternalWeatherApiOptions> options)
        {
            _httpClient = httpClientFactory.CreateClient() ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Gets current weather data from external weather api.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<WeatherData> GetCurrentWeather(string location, CancellationToken cancellationToken)
        {
            string apiBaseUrl = _options.Value.ApiBaseUrl;
            string apiEndpoint = _options.Value.ApiEndpoint;
            string apiKey = _options.Value.ApiKey;

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{apiBaseUrl}/{apiEndpoint}?q={location}&appid={apiKey}")
        };

            using (HttpResponseMessage response = await _httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false))
            {
                string resultContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<WeatherData>(resultContent, new JsonSerializerSettings());
            }
        }
    }
}
