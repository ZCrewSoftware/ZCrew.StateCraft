namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class CanTransitionTests_T1_T2_T3_T4
{
    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenTransitionExists_ShouldReturnTrue()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition(
            "To B",
            42,
            "hello",
            true,
            3.14,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenTransitionDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition(
            "To C",
            42,
            "hello",
            true,
            3.14,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenNotActivated_ShouldReturnFalse()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        // Act
        var result = await stateMachine.CanTransition(
            "To B",
            42,
            "hello",
            true,
            3.14,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenConditionIsTrue_ShouldReturnTrue()
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
                        t => t.WithParameters<int, string, bool, double>().If((_, _, _, _) => true).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition(
            "To B",
            42,
            "hello",
            true,
            3.14,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_WhenConditionIsFalse_ShouldReturnFalse()
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
                        t => t.WithParameters<int, string, bool, double>().If((_, _, _, _) => false).To("B")
                    )
            )
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var result = await stateMachine.CanTransition(
            "To B",
            42,
            "hello",
            true,
            3.14,
            TestContext.Current.CancellationToken
        );

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanTransition_T1_T2_T3_T4_ShouldNotModifyStateMachineState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A")
            .WithState("A", state => state.WithTransition<int, string, bool, double>("To B", "B"))
            .WithState("B", state => state.WithParameters<int, string, bool, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.CanTransition("To B", 42, "hello", true, 3.14, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("A", stateMachine.CurrentState.StateValue);
    }
}
