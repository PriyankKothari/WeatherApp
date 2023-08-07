namespace WeatherApp.OpenWeatherMapApi.Client.Options
{
    /// <summary>
    /// External Weather Api Options
    /// </summary>
    public sealed class ExternalWeatherApiOptions
    {
        /// <summary>
        /// Gets or sets Api BaseUrl.
        /// </summary>
        public string ApiBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Api Endpoint.
        /// </summary>
        public string ApiEndpoint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets Api Key.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
