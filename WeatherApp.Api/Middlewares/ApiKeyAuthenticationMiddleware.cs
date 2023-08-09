using System.Net;

namespace WeatherApp.Api.Middlewares
{
    /// <summary>
    /// ApiKey Authentication middleware.
    /// </summary>
    public class ApiKeyAuthenticationMiddleware
    {
        private const string ApiKeysSectionName = "Authentication:ApiKeys";
        private const string ApiKeyHeaderName = "X-API-KEY";

        private readonly RequestDelegate _nextRequestDelegate;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

        /// <summary>
        /// Initializes an instance of the <see cref="ApiKeyAuthenticationMiddleware" /> class.
        /// </summary>
        /// <param name="nextRequestDelegate">Next <see cref="RequestDelegate" />.</param>
        /// <param name="configuration"><see cref="IConfiguration" />.</param>
        /// <param name="logger"><see cref="ILogger{ApiKeyAuthenticationMiddleware}" />.</param>
        public ApiKeyAuthenticationMiddleware(RequestDelegate nextRequestDelegate, IConfiguration configuration, ILogger<ApiKeyAuthenticationMiddleware> logger)
        {
            _nextRequestDelegate = nextRequestDelegate ?? throw new ArgumentNullException(nameof(nextRequestDelegate));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invoke the authentication for a given <see cref="HttpContext" />.
        /// </summary>
        /// <param name="context"><see cref="HttpContext" />.</param>
        /// <returns><see cref="HttpStatusCode.Unauthorized" /> if the ApiKey is missing or invalid. Otherwise, continue to the next <see cref="RequestDelegate" />.</returns>
        public async Task Invoke(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogCritical("Authentication failed. API KEY is not provided with the request.");

                // set StatusCode from Enum instead of integer value for readability
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("MISSING API KEY");
                return;
            }

            List<string> apiKeys = _configuration.GetSection($"{ApiKeysSectionName}:{ApiKeyHeaderName}").Get<List<string>>();

            // Check each api key with OrdinalIgnoreCase
            if (!apiKeys.Any(apiKey => apiKey.Equals(extractedApiKey, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogCritical("Authentication failed. Valid API KEY is not provided with the request.");

                // set StatusCode from Enum instead of integer value for readability
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync($"INVALID API KEY");
                return;
            }

            await _nextRequestDelegate(context);
        }
    }
}