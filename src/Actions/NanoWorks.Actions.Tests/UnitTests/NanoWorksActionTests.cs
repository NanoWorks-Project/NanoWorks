// Ignore Spelling: Nano

using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using NanoWorks.Actions.Results;
using Shouldly;

namespace NanoWorks.Actions.Tests.UnitTests;

public class NanoWorksActionTests
{
    private readonly IFixture _fixture;
    private readonly IEnumerable<Mock<IActionStep<string, string>>> _mockSteps;
    private readonly NanoWorksAction<string, string> _nanoWorksAction;

    public NanoWorksActionTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });

        _mockSteps = _fixture.CreateMany<Mock<IActionStep<string, string>>>();

        foreach (var mock in _mockSteps)
        {
            mock.Setup(step => step.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ActionResult.Continue());
        }

        _nanoWorksAction = new NanoWorksAction<string, string>(_mockSteps.Select(mock => mock.Object));
    }

    [Test]
    public async Task ExecuteAsync_ShouldExecuteSteps_AndReturnResponse()
    {
        // arrange
        var expectedResponse = _fixture.Create<string>();

        var lastStep = _mockSteps.Last();

        lastStep.Setup(step => step.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ActionResult.Complete(expectedResponse));

        var request = _fixture.Create<string>();

        // act
        var response = await _nanoWorksAction.ProcessAsync(request, CancellationToken.None);

        // assert
        response.ShouldBe(expectedResponse);

        foreach (var mock in _mockSteps)
        {
            mock.Verify(step => step.ExecuteAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
