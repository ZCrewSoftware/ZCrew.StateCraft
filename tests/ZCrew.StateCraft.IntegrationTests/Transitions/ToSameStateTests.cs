using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class ToSameStateTests
{
    [Fact]
    public async Task Transition_ToSameState_ShouldRemainInSameState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("Loop", t => t.WithNoParameters().ToSameState()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_ToSameState_ShouldCallExitThenStateChangeThenEntry()
    {
        // Arrange
        var onExit = Substitute.For<Action>();
        var onStateChange = Substitute.For<Action<string, string, string>>();
        var onEntry = Substitute.For<Action>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state
                        .OnExit(onExit)
                        .OnStateChange(onStateChange)
                        .OnEntry(onEntry)
                        .WithTransition("Loop", t => t.WithNoParameters().ToSameState())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onEntry.Received(1).Invoke();
            onExit.Received(1).Invoke();
            onStateChange.Received(1).Invoke("A", "Loop", "A");
            onEntry.Received(1).Invoke();
        });
    }

    [Fact]
    public async Task Transition_ToSameStateFromParameterizedState_ShouldRemainInSameState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state => state.WithParameter<int>().WithTransition("Loop", t => t.WithSameParameter().ToSameState())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_ToSameStateFromParameterizedState_ShouldCallOnExitWithParameter()
    {
        // Arrange
        var onExit = Substitute.For<Action<int>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .OnExit(onExit)
                        .WithTransition("Loop", t => t.WithSameParameter().ToSameState())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42);
    }

    [Fact]
    public async Task Transition_ToSameState_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("Loop", t => t.WithNoParameters().If(() => true).ToSameState())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_ToSameState_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state => state.WithTransition("Loop", t => t.WithNoParameters().If(() => false).ToSameState())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("Loop", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
