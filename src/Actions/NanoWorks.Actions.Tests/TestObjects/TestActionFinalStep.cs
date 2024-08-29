// Ignore Spelling: Nano

using NanoWorks.Actions.Results;

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionFinalStep : IActionStep<string, string>
{
    public const string ExpectedResponse = "Final Step Response";

    private static readonly List<TestActionStepInvocation> _invocations = new();
    public static IEnumerable<TestActionStepInvocation> Invocations => _invocations;

    public async Task<ActionResult> ExecuteAsync(string request, CancellationToken cancellationToken)
    {
        _invocations.Add(new TestActionStepInvocation(request, cancellationToken));
        return ActionResult.Complete(ExpectedResponse);
    }
}
