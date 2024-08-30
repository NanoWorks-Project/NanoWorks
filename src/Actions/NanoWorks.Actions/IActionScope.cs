namespace NanoWorks.Actions;

/// <summary>
/// Scope for an action containing the request and response, and values that can be shared between steps.
/// </summary>
/// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
/// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
public interface IActionScope<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Gets the request passed to the action.
    /// </summary>
    TRequest Request { get; }

    /// <summary>
    /// Gets or sets the response returned by the action.
    /// </summary>
    TResponse Response { get; set; }

    /// <summary>
    /// Gets a value from the action scope.
    /// </summary>
    /// <typeparam name="TValue">Type of value to get.</typeparam>
    TValue Get<TValue>();

    /// <summary>
    /// Sets a value in the action scope.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <typeparam name="TValue">Type of value to set.</typeparam>
    void Set<TValue>(TValue value);
}
