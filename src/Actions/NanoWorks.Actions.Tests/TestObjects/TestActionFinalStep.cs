// Ignore Spelling: Nano

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionFinalStep : IActionStep<string, string>
{
    public const string ExpectedResponse = "Final Step Response";

    private static readonly List<TestActionStepInvocation> _invocations = [];
    public static IEnumerable<TestActionStepInvocation> Invocations => _invocations;

    public Task ExecuteAsync(IActionScope<string, string> scope, CancellationToken cancellationToken)
    {
        _invocations.Add(new TestActionStepInvocation(scope, cancellationToken));
        scope.Response = ExpectedResponse;
        return Task.CompletedTask;
    }
}
