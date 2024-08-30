// Ignore Spelling: Nano

using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.DependencyInjection;
using NanoWorks.Actions.DependencyInjection;
using NanoWorks.Actions.Tests.TestObjects;
using Shouldly;

namespace NanoWorks.Actions.Tests.IntegrationTests;

public class NanoWorksActionTests : IDisposable
{
    private readonly IFixture _fixture;
    private readonly IServiceScope _serviceScope;
    private readonly IAction<string, string> _action;

    public NanoWorksActionTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });

        var services = new ServiceCollection();

        services.AddAction<string, string>(options =>
        {
            options.AddStep<TestActionStep>();
            options.AddStep<TestActionStep>();
            options.AddStep<TestActionStep>();
            options.AddStep<TestActionFinalStep>();
        });

        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();

        _serviceScope = serviceProvider.CreateScope();
        _action = _serviceScope.ServiceProvider.GetRequiredService<IAction<string, string>>();
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }

    [Test]
    public async Task ProcessAsync_Should_ReturnResponse_And_ExecuteSteps()
    {
        // arrange
        var request = _fixture.Create<string>();

        // act
        var response = await _action.ProcessAsync(request, CancellationToken.None);

        // assert
        response.ShouldBe(TestActionFinalStep.ExpectedResponse);

        TestActionStep.Invocations.Count().ShouldBe(3);
        TestActionStep.Invocations.All(i => i.Scope.Request == request).ShouldBeTrue();
        TestActionStep.Invocations.All(i => i.CancellationToken == CancellationToken.None).ShouldBeTrue();

        TestActionFinalStep.Invocations.Count().ShouldBe(1);
        TestActionFinalStep.Invocations.All(i => i.Scope.Request == request).ShouldBeTrue();
        TestActionFinalStep.Invocations.All(i => i.CancellationToken == CancellationToken.None).ShouldBeTrue();
    }
}
