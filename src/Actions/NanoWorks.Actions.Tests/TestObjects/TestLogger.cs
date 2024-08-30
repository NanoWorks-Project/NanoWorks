// Ignore Spelling: Nano

using Microsoft.Extensions.Logging;

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestLogger<T> : ILogger<T>
{
    private readonly static List<object?[]> _invocationArguments = [];

    internal static List<object?[]> InvocationArguments => _invocationArguments;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _invocationArguments.Add([logLevel, eventId, state, exception, formatter]);
    }
}
