// Ignore Spelling: Nano

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionStepInvocation
{
    public TestActionStepInvocation(IActionScope<string, string> scope, CancellationToken cancellationToken)
    {
        Scope = scope;
        CancellationToken = cancellationToken;
    }

    internal IActionScope<string, string> Scope { get; init; }
    internal CancellationToken CancellationToken { get; init; }
}
