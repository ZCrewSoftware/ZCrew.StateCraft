namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class CanTransitionTests_T1_T2_T3
{
    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenTransitionExists_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenTransitionDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To C", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenNotActivated_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenConditionIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => true).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenConditionIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState(
                "A",
                state =>
                    state.WithTransition(
                        "To B",
                        t => t.WithParameters<int, string, bool>().If((_, _, _) => false).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenFromParameterizedState_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState("A", state => state.WithParameter<int>().WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenPreWhenConditionOnNextIsTrue_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.WithParameters<int, string, bool>().If((i, s, _) => i > 0 && s.Length > 0).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_WhenPreWhenConditionOnNextIsFalse_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 10)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameter<int>()
                        .WithTransition(
                            "To B",
                            t => t.WithParameters<int, string, bool>().If((i, _, _) => i < 0).To("B")
                        )
            )
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_ShouldNotModifyStateMachineState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.CanTransition("To B", 42, "hello", true, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
