using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace WeatherApp.Api.Middlewares
{
    /// <summary>
    /// Custom Rate Limiting Configuration.
    /// </summary>
    internal class CustomClientRateLimitingMiddleware : ClientRateLimitMiddleware
    {
        /// <summary>
        /// Initializes a new instalces of the <see cref="CustomClientRateLimitingMiddleware" /> class.
        /// </summary>
        /// <param name="nextRequestDelegate"><see cref="RequestDelegate" />.</param>
        /// <param name="processingStrategy"><see cref="IProcessingStrategy" />.</param>
        /// <param name="clientRateLimitOptions"><see cref="ClientRateLimitOptions" />.</param>
        /// <param name="clientPolicyStore"><see cref="IClientPolicyStore" />.</param>
        /// <param name="rateLimitConfiguration"><see cref="IRateLimitConfiguration" />.</param>
        /// <param name="logger"><see cref="ILogger{ClientRateLimitMiddleware}" />.</param>
        public CustomClientRateLimitingMiddleware(
            RequestDelegate nextRequestDelegate,
            IProcessingStrategy processingStrategy,
            IOptions<ClientRateLimitOptions> clientRateLimitOptions,
            IClientPolicyStore clientPolicyStore,
            IRateLimitConfiguration rateLimitConfiguration,
            ILogger<ClientRateLimitMiddleware> logger)
            : base(nextRequestDelegate, processingStrategy, clientRateLimitOptions, clientPolicyStore, rateLimitConfiguration, logger)
        {
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            int.TryParse(retryAfter, out int retryAfterInSeconds);

            var message = new
            {
                data = default(object),
                errors = new List<string> { $"Limit exceeded: Maximum {rule.Limit} requests allowed per {rule.Period}. Try again in {retryAfterInSeconds / 60} minute(s)." }
            };

            httpContext.Response.Headers["Retry-After"] = retryAfter;
            httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            httpContext.Response.ContentType = "application/json";

            return Task.FromResult(httpContext.Response.WriteAsync(JsonConvert.SerializeObject(message)));
        }
    }
}
