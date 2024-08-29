// Ignore Spelling: Nano

using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Actions;

/// <summary>
/// Definition of an action.
/// </summary>
/// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
/// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
public interface IAction<TRequest, TResponse>
{
    /// <summary>
    /// Processes the request and returns a response.
    /// </summary>
    /// <param name="request">Request passed to the action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken);
}
