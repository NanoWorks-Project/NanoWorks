// Ignore Spelling: Nano

using System;
using System.Collections.Generic;

namespace NanoWorks.Actions;

internal class NanoWorksActionScope<TRequest, TResponse> : IActionScope<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly Dictionary<Type, object> _values = new();

    public NanoWorksActionScope(TRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        Request = request;
    }

    public TRequest Request { get; }

    public TResponse Response { get; set; } = null!;

    public TValue Get<TValue>()
    {
        var value = (TValue)_values[typeof(TValue)];

        if (value is null)
        {
            throw new InvalidOperationException($"Value of type {typeof(TValue).Name} not found in action scope.");
        }

        return value;
    }

    public void Set<TValue>(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        _values[typeof(TValue)] = value;
    }
}
