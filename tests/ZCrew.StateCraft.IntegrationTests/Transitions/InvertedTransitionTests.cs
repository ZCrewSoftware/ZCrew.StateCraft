namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class InvertedTransitionTests
{
    [Fact]
    public async Task Transition_WithAllOtherStates_WhenCalledFromSourceState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithAllOtherStates_WhenCalledFromDifferentSourceState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithAllOtherStates_WhenCalledFromDestinationState_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To D", "D"))
            .WithState("D", state => state.WithTransition("Reenter D", t => t.From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("Reenter D", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithAllStates_WhenCalledFromSourceState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithAllStates_WhenCalledFromDestinationState_ShouldReenterState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To D", "D"))
            .WithState("D", state => state.WithTransition("Reenter D", t => t.From().AllStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("Reenter D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithExcept_WhenCalledFromExcludedState_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("C")
            .WithState("A", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("C")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task Transition_WithExcept_WhenCalledFromNonExcludedState_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("C", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("C")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithMultipleExcept_WhenCalledFromAnyExcludedState_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("B")
            .WithState("A", state => state)
            .WithState("B", state => state)
            .WithState("C", state => state)
            .WithState(
                "D",
                state => state.WithTransition("To D", t => t.From().AllOtherStates().Except("B").Except("C"))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }

    [Fact]
    public async Task CanTransition_WithExceptT_WhenCalledFromExcludedParameterizedState_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except<int>("B")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);
        await stateMachine.Transition("To B", 42, TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_WithExceptT_WhenCalledFromNonMatchingState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("B")
            .WithState("B", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.From().AllOtherStates().Except<int>("B")))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Transition_WithConditionBeforeFrom_WhenConditionIsTrue_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state)
            .WithState("D", state => state.WithTransition("To D", t => t.If(() => true).From().AllOtherStates()))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To D", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("D", stateMachine.CurrentState.StateValue);
    }
}
