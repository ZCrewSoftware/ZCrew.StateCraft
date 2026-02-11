namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class CanTransitionTests_T
{
    [Fact]
    public async Task CanTransition_T_WhenTransitionExists_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenTransitionDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To C", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenNotActivated_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        // Act
        var result = await stateMachine.CanTransition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.WithParameter<int>().If(x => x > 0).To("B")))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.WithParameter<int>().If(x => x < 0).To("B")))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenFromParameterizedState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition<string>("To B", "B"))
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenPreWhenConditionOnNextIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(next => next.Length > 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T_WhenPreWhenConditionOnNextIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition("To B", t => t.WithParameter<string>().If(next => next.Length == 0).To("B"))
            )
            .WithState("B", state => state.WithParameter<string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", "hello", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T_ShouldNotModifyStateMachineState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int>("To B", "B"))
            .WithState("B", state => state.WithParameter<int>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.CanTransition("To B", 42, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
