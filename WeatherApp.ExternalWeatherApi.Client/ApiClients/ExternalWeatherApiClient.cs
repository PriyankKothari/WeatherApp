using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherApp.ExternalWeatherApi.Client.Options;

namespace WeatherApp.ExternalWeatherApi.Client.ApiClients
{
    /// <summary>
    /// Implements methods communicating with the external weather api.
    /// </summary>
    public sealed class ExternalWeatherApiClient : IExternalWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalWeatherApiClient> _logger;

        private readonly string _apiBaseUrl;
        private readonly string _apiEndpoint;
        private readonly string _apiKey;

        /// <summary>
        /// Initiates a new instance of the <see cref="ExternalWeatherApiClient" /> class.
        /// </summary>
        /// <param name="httpClientFactory"><see cref="IHttpClientFactory" />.</param>
        /// <param name="options"><see cref="IOptions{ExternalWeatherApiOptions}" />.</param>
        public ExternalWeatherApiClient(IHttpClientFactory httpClientFactory, ILogger<ExternalWeatherApiClient> logger, IOptions<ExternalWeatherApiOptions> options)
        {
            ArgumentNullException.ThrowIfNull(httpClientFactory, nameof(httpClientFactory));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _apiBaseUrl = options.Value.ApiBaseUrl;
            _apiEndpoint = options.Value.ApiEndpoint;
            _apiKey = options.Value.ApiKey;
        }

        /// <summary>
        /// Gets current weather data from external weather api.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken" />.</param>
        /// <returns><see cref="HttpResponseMessage" />.</returns>
        public async Task<HttpResponseMessage> GetCurrentWeather(string location, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(location, nameof(location));

            HttpResponseMessage? response;
            try
            {
                Uri requestUri = new Uri($"{_apiBaseUrl}/{_apiEndpoint}?q={location}&appid={_apiKey}");

                // log http request message
                _logger.LogInformation($"An external weather api endpoint is requested.", requestUri);

                response = await _httpClient.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

                // log http response message
                _logger.LogInformation("Response recieved.", response);
            }
            catch (Exception exception)
            {
                _logger.LogError("An error occured while requesting to an external weather api endpoint.", exception);
                response = new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.InternalServerError, ReasonPhrase = exception.Message };
            }

            return response;
        }
    }
}
