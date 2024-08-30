using System.Threading;
using System.Threading.Tasks;

namespace NanoWorks.Actions;

/// <summary>
/// Defines a step of an action.
/// </summary>
/// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
/// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
public interface IActionStep<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Executes the action step.
    /// </summary>
    /// <param name="scope">Scope for the action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ExecuteAsync(IActionScope<TRequest, TResponse> scope, CancellationToken cancellationToken);
}
