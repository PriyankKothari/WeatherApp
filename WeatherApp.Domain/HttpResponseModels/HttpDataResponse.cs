using System.Net;

namespace WeatherApp.Domain.HttpResponseModels
{
    /// <summary>
    /// Http Data Response.
    /// </summary>
    /// <typeparam name="T"><see cref="T" />.</typeparam>
    public sealed class HttpDataResponse<T>
    {
        /// <summary>
        /// Gets or sets the Status Code.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets Data.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets Errors.
        /// </summary>
        public ICollection<string>? Errors { get; set; } = new HashSet<string>();
    }
}
