using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class MappedTransitionStaleParameterTests
{
    [Fact(Skip = "BL-F66: Post-condition failure leaves stale mapped parameter in next slot")]
    public async Task MappedTransition_WhenPostConditionFails_ShouldNotBlockSubsequentMappedTransition()
    {
        // Arrange — A(int) has two mapped transitions:
        // 1. "To B" maps int→string with always-false post-condition
        // 2. "To C" maps int→int
        var secondMapping = Substitute.For<Func<int, int>>();
        secondMapping.Invoke(Arg.Any<int>()).Returns(99);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.WithMappedParameter<string>(i => i.ToString()).If(_ => false).To("B")
                        )
                        .WithTransition("To C", t => t.WithMappedParameter<int>(secondMapping).To("C"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .WithState("C", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // First attempt — post-condition fails, leaving stale string in Next slot
        var result = await stateMachine.TryTransition("To B", TestContext.Current.CancellationToken);
        Assert.False(result);

        // Act — second transition should find "To C" mapping, not be blocked by stale parameter
        await stateMachine.Transition("To C", TestContext.Current.CancellationToken);

        // Assert
        secondMapping.Received(1).Invoke(42);
    }

    [Fact(Skip = "BL-F66: CanTransition leaves stale mapped parameter preventing second mapping invocation")]
    public async Task CanTransition_WhenCalledTwiceOnMappedTransition_ShouldInvokeMappingBothTimes()
    {
        // Arrange
        var mapping = Substitute.For<Func<int, string>>();
        mapping.Invoke(Arg.Any<int>()).Returns("mapped");

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithMappedParameter<string>(mapping).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);
        await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert — mapping should be invoked on both calls
        mapping.Received(2).Invoke(42);
    }
}
