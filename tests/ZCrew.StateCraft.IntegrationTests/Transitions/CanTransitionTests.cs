namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class CanTransitionTests
{
    [Fact]
    public async Task CanTransition_WhenTransitionExists_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenTransitionDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To C", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_WhenNotActivated_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_WhenConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.If(() => true).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", t => t.If(() => false).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_WhenFromParameterizedState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenPreConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.If(x => x > 0).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_WhenPreConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42)
            .WithState("A", state => state.WithParameter<int>().WithTransition("To B", t => t.If(x => x < 0).To("B")))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_ShouldNotModifyStateMachineState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition("To B", "B"))
            .WithState("B", state => state)
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.CanTransition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
