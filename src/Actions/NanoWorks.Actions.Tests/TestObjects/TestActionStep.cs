﻿// Ignore Spelling: Nano
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using NanoWorks.Actions.Results;

namespace NanoWorks.Actions.Tests.TestObjects;

public class TestActionStep : IActionStep<string, string>
{
    private static readonly List<TestActionStepInvocation> _invocations = [];
    public static IEnumerable<TestActionStepInvocation> Invocations => _invocations;

    public async Task<ActionResult> ExecuteAsync(string request, CancellationToken cancellationToken)
    {
        _invocations.Add(new TestActionStepInvocation(request, cancellationToken));
        return ActionResult.Continue();
    }
}
