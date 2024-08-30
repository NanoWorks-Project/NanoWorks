// Ignore Spelling: Nano

namespace NanoWorks.Actions;

internal class NanoWorksActionScopeProvider : IActionScopeProvider
{
    public IActionScope<TRequest, TResponse> CreateScope<TRequest, TResponse>(TRequest request)
        where TRequest : class
        where TResponse : class
    {

        var scope = new NanoWorksActionScope<TRequest, TResponse>(request);
        return scope;
    }
}
