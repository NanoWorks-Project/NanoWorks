// Ignore Spelling: Nano

namespace NanoWorks.Actions;

/// <summary>
/// Action scope provider.
/// </summary>
public interface IActionScopeProvider
{
    /// <summary>
    /// Create a new action scope.
    /// </summary>
    /// <typeparam name="TRequest">Request type.</typeparam>
    /// <typeparam name="TResponse">Response type.</typeparam>
    /// <param name="request">Action request.</param>
    IActionScope<TRequest, TResponse> CreateScope<TRequest, TResponse>(TRequest request)
        where TRequest : class
        where TResponse : class;
}
