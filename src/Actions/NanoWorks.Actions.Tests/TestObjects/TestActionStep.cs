// Ignore Spelling: Nano
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionStep : IActionStep<string, string>
{
    private static readonly List<TestActionStepInvocation> _invocations = [];
    public static IEnumerable<TestActionStepInvocation> Invocations => _invocations;

    public Task ExecuteAsync(IActionScope<string, string> scope, CancellationToken cancellationToken)
    {
        _invocations.Add(new TestActionStepInvocation(scope, cancellationToken));
        return Task.CompletedTask;
    }
}
