// Ignore Spelling: Nano

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionStepInvocation
{
    public TestActionStepInvocation(
        string request,
        CancellationToken cancellationToken)
    {
        Request = request;
        CancellationToken = cancellationToken;
    }

    internal string Request { get; init; }
    internal CancellationToken CancellationToken { get; init; }
}
