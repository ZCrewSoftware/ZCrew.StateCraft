using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithSameParametersTests_T1_T2_T3
{
    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldPreserveParameterValues()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", 3.14);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldCallOnExitWithParameters()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello", 3.14);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldCallOnEntryWithSameParameters()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello", 3.14);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldCallOnStateChangeWithParameters()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello", 3.14);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string, double>>();
        var onEntry = Substitute.For<Action<int, string, double>>();
        var invoke = Substitute.For<Action<int, string, double>>();
        var onExit = Substitute.For<Action<int, string, double>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .OnStateChange(onStateChangeMachine)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .OnStateChange(onStateChangeState)
                        .OnEntry(onEntry)
                        .WithAction(action => action.Invoke(invoke))
            )
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Received.InOrder(() =>
        {
            onExit.Received(1).Invoke(42, "hello", 3.14);
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "hello", 3.14);
            onEntry.Received(1).Invoke(42, "hello", 3.14);
            invoke.Received(1).Invoke(42, "hello", 3.14);
        });
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().If((x, _, _) => x > 0).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_T3_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello", 3.14)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string, double>()
                        .WithTransition("To B", t => t.WithSameParameters().If((_, _, _) => false).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string, double>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
