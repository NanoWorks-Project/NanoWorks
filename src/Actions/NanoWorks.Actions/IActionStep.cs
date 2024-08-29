using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Actions.Results;

namespace NanoWorks.Actions;

/// <summary>
/// Defines a step of an action.
/// </summary>
/// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
/// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
public interface IActionStep<TRequest, TResponse>
{
    /// <summary>
    /// Executes the action step.
    /// </summary>
    /// <param name="request">Request passed to the action.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ActionResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
