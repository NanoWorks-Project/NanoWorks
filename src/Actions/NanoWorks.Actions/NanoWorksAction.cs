// Ignore Spelling: Nano

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NanoWorks.Actions.Results;

namespace NanoWorks.Actions;

internal class NanoWorksAction<TRequest, TResponse>(IEnumerable<IActionStep<TRequest, TResponse>> steps)
    : IAction<TRequest, TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken)
    {
        foreach (var step in steps)
        {
            var result = await step.ExecuteAsync(request, cancellationToken);

            if (result is ActionCompleted<TResponse> actionCompletedResult)
            {
                return actionCompletedResult.Response;
            }
        }

        throw new InvalidOperationException("Action did not complete successfully.");
    }
}
