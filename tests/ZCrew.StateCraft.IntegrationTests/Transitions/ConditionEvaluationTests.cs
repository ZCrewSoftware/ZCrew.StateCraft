using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class ConditionEvaluationTests
{
    [Fact]
    public async Task Transition_WhenFirstConditionIsFalse_ShouldNotEvaluateSecondCondition()
    {
        // Arrange
        var secondCondition = Substitute.For<Func<bool>>();
        secondCondition.Invoke().Returns(true);

        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("To B", t => t.If(() => false).If(secondCondition).To("B"))
            )
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        secondCondition.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task Transition_WhenPreviousStageConditionFails_ShouldNotEvaluateNextStageCondition()
    {
        // Arrange — mapped transition with pre-condition that fails;
        // mapping and post-condition should NOT be invoked
        var mapping = Substitute.For<Func<int, string>>();
        mapping.Invoke(Arg.Any<int>()).Returns("mapped");

        var postCondition = Substitute.For<Func<string, bool>>();
        postCondition.Invoke(Arg.Any<string>()).Returns(true);

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
                            t => t.If(_ => false).WithMappedParameter<string>(mapping).If(postCondition).To("B")
                        )
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
        mapping.DidNotReceive().Invoke(Arg.Any<int>());
        postCondition.DidNotReceive().Invoke(Arg.Any<string>());
    }
}
