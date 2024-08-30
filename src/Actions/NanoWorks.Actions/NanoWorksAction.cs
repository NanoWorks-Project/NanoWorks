// Ignore Spelling: Nano
#pragma warning disable SA1402 // File may only contain a single type

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NanoWorks.Actions;

internal class NanoWorksAction
{
}

internal class NanoWorksAction<TRequest, TResponse>(
    IEnumerable<IActionStep<TRequest, TResponse>> steps,
    IActionScopeProvider scopeProvider,
    ILogger<NanoWorksAction> logger)
    : IAction<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    /// <inheritdoc />
    public async Task<TResponse> ProcessAsync(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Processing request '{typeof(TRequest).Name}'.");

        var scope = scopeProvider.CreateScope<TRequest, TResponse>(request);

        foreach (var step in steps)
        {
            await TryExecuteStep(scope, step, cancellationToken);

            if (scope.Response is null)
            {
                continue;
            }

            logger.LogInformation($"Processing request '{typeof(TRequest).Name}' complete.");
            return scope.Response;
        }

        var errorMessage = $"Action did not complete successfully after processing request '{typeof(TRequest).Name}' - no response set '{typeof(TResponse).Name}'.";
        logger.LogError(errorMessage);
        throw new InvalidOperationException(errorMessage);
    }

    private async Task TryExecuteStep(IActionScope<TRequest, TResponse> scope, IActionStep<TRequest, TResponse> step, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation($"Executing step {step.GetType().Name}.");

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            await step.ExecuteAsync(scope, cancellationToken);
            stopWatch.Stop();

            logger.LogInformation($"Step {step.GetType().Name} executed in {stopWatch.ElapsedMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            var errorMessage = $"An unexpected error occurred in step '{step.GetType().Name}' while processing request '{typeof(TRequest).Name}'.";
            logger.LogError(ex, errorMessage);
            throw new Exception(errorMessage, ex);
        }
    }
}
