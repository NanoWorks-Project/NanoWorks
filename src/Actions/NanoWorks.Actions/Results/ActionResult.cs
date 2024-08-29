// Ignore Spelling: Nano
#pragma warning disable SA1402 // File may only contain a single type

namespace NanoWorks.Actions.Results;

/// <summary>
/// Result from an action step.
/// </summary>
public class ActionResult
{
    internal ActionResult()
    {
    }

    /// <summary>
    /// Return a result to complete the action.
    /// </summary>
    /// <typeparam name="TResponse">Type of response returned from the action.</typeparam>
    /// <param name="response">Response to return from the action.</param>
    public static ActionCompleted<TResponse> Complete<TResponse>(TResponse response)
    {
        return new ActionCompleted<TResponse>(response);
    }

    /// <summary>
    /// Return a result to continue the action.
    /// </summary>
    public static ContinueAction Continue()
    {
        return new ContinueAction();
    }
}

/// <summary>
/// Complete result from an action step.
/// </summary>
/// <typeparam name="TResponse">Type of response returned from the action.</typeparam>
public class ActionCompleted<TResponse> : ActionResult
{
    internal ActionCompleted(TResponse response)
    {
        Response = response;
    }

    /// <summary>
    /// Gets the response returned from the action.
    /// </summary>
    public TResponse Response { get; private set; }
}

/// <summary>
/// Continue result from an action step.
/// </summary>
public class ContinueAction : ActionResult
{
    internal ContinueAction()
    {
    }
}
