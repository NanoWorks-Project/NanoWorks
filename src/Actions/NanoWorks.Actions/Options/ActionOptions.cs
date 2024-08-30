// Ignore Spelling: Nano

using System;
using System.Collections.Generic;

namespace NanoWorks.Actions.Options;

/// <summary>
/// Options for an action.
/// </summary>
/// <typeparam name="TRequest">Type of request passed to the action.</typeparam>
/// <typeparam name="TResponse">Type of response returned by the action.</typeparam>
public class ActionOptions<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly LinkedList<Type> _steps = new();

    internal IEnumerable<Type> ProcessingSteps => _steps;

    /// <summary>
    /// Adds a step to the action.
    /// </summary>
    /// <typeparam name="TStep">Type of step.</typeparam>
    public ActionOptions<TRequest, TResponse> AddStep<TStep>()
        where TStep : IActionStep<TRequest, TResponse>
    {
        _steps.AddLast(typeof(TStep));
        return this;
    }
}
