using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;

namespace WeatherApp.Api.Middlewares
{
    /// <summary>
    /// Custom Rate Limiting Configuration.
    /// </summary>
    internal class CustomRateLimitingConfiguration : RateLimitConfiguration
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        /// <summary>
        /// Initializes a new instalces of the <see cref="CustomRateLimitingConfiguration" /> class.
        /// </summary>
        /// <param name="ipRateLimitOptions"><see cref="IpRateLimitOptions" />.</param>
        /// <param name="clientRateLimitOptions"><see cref="ClientRateLimitOptions" />.</param>
        public CustomRateLimitingConfiguration(IOptions<IpRateLimitOptions> ipRateLimitOptions, IOptions<ClientRateLimitOptions> clientRateLimitOptions)
            : base(ipRateLimitOptions, clientRateLimitOptions)
        {
        }

        /// <summary>
        /// Register custom resolver.
        /// </summary>
        public override void RegisterResolvers()
        {
            base.RegisterResolvers();
            ClientResolvers.Add(new CustomClientResolveContributor());
        }

        /// <summary>
        /// Custom Client Resolve Contributor.
        /// </summary>
        private class CustomClientResolveContributor : IClientResolveContributor
        {
            public Task<string> ResolveClientAsync(HttpContext httpContext)
            {
                string headerValue = string.Empty;

                if (httpContext.Response.Headers.TryGetValue(ApiKeyHeaderName, out var values))
                {
                    Task.FromResult(values.ToString());
                }

                return Task.FromResult(headerValue);
            }
        }
    }
}
