namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class InvertedTransitionTests_T
{
    [Fact]
    public async Task Transition_T_WithAllOtherStates_WhenCalledFromSourceState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("D", state => state.WithParameter<int>().WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task CanTransition_T_WithAllOtherStates_WhenCalledFromSourceState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithParameter<int>().WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T_WithAllOtherStates_WhenCalledFromDestinationState_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To D", "D"))
            .WithState(
                "D",
                state => state.WithParameter<int>().WithTransition("Reenter D", t => t.From().AllOtherStates())
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To D", 42, TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("Reenter D", 99, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TryTransition_T_WithAllOtherStates_WhenCalledFromSourceState_ShouldReturnTrueAndChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithParameter<int>().WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.TryTransition("To D", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }
}
