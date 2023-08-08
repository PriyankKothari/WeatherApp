using WeatherApp.Domain.HttpResponseModels;

namespace WeatherApp.Application.Abstrations
{
    /// <summary>
    /// Represents a generic request handler interface.
    /// </summary>
    /// <typeparam name="TRequest">A <see cref="TRequest" />.</typeparam>
    /// <typeparam name="TResponse">A <see cref="TResponse" />.</typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
    {
        /// <summary>
        /// Handles request.
        /// </summary>
        /// <param name="request"><typeparamref name="TRequest" />.</param>
        /// <returns><typeparamref name="TResponse" />.</returns>
        Task<HttpDataResponse<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
