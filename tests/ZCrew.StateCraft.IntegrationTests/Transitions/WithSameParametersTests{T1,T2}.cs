using NSubstitute;

namespace ZCrew.StateCraft.IntegrationTests.Transitions;

public class WithSameParametersTests_T1_T2
{
    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldChangeState()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state => state.WithParameters<int, string>().WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldPreserveParameterValues()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state => state.WithParameters<int, string>().WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldCallOnExitWithParameters()
    {
        // Arrange
        var onExit = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onExit.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldCallOnEntryWithSameParameters()
    {
        // Arrange
        var onEntry = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state => state.WithParameters<int, string>().WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>().OnEntry(onEntry))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onEntry.Received(1).Invoke(42, "hello");
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldCallOnStateChangeWithParameters()
    {
        // Arrange
        var onStateChange = Substitute.For<Action<string, string, string, int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state => state.WithParameters<int, string>().WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>().OnStateChange(onStateChange))
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        onStateChange.Received(1).Invoke("A", "To B", "B", 42, "hello");
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_ShouldCallMethodsInCorrectOrder()
    {
        // Arrange
        var onStateChangeMachine = Substitute.For<Action<string, string, string>>();
        var onStateChangeState = Substitute.For<Action<string, string, string, int, string>>();
        var onEntry = Substitute.For<Action<int, string>>();
        var invoke = Substitute.For<Action<int, string>>();
        var onExit = Substitute.For<Action<int, string>>();
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .OnStateChange(onStateChangeMachine)
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .OnExit(onExit)
                        .WithTransition("To B", t => t.WithSameParameters().To("B"))
            )
            .WithState(
                "B",
                state =>
                    state
                        .WithParameters<int, string>()
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
            onExit.Received(1).Invoke(42, "hello");
            onStateChangeMachine.Received(1).Invoke("A", "To B", "B");
            onStateChangeState.Received(1).Invoke("A", "To B", "B", 42, "hello");
            onEntry.Received(1).Invoke(42, "hello");
            invoke.Received(1).Invoke(42, "hello");
        });
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_WhenConditionIsTrue_ShouldTransition()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithSameParameters().If((x, _) => x > 0).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        await stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Equal("B", stateMachine.CurrentState.StateValue);
    }

    [Fact]
    public async Task Transition_WithSameParameters_T1_T2_WhenConditionIsFalse_ShouldThrow()
    {
        // Arrange
        var stateMachine = StateMachine
            .Configure<string, string>()
            .WithInitialState("A", 42, "hello")
            .WithState(
                "A",
                state =>
                    state
                        .WithParameters<int, string>()
                        .WithTransition("To B", t => t.WithSameParameters().If((_, _) => false).To("B"))
            )
            .WithState("B", state => state.WithParameters<int, string>())
            .Build();

        await stateMachine.Activate(TestContext.Current.CancellationToken);

        // Act
        var transition = () => stateMachine.Transition("To B", TestContext.Current.CancellationToken);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(transition);
    }
}
